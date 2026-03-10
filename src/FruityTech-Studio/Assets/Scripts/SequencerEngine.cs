using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SequencerEngine : MonoBehaviour
{
    // Captures an instrument's full UI/audio state so delete can be undone
    public sealed class InstrumentState
    {
        public string InstrumentName;
        public List<NoteEvent> Notes = new();
        public float Volume;
        public bool Muted;
        public bool WasActiveInstrument;
        public bool WasVisible;
    }

    private sealed class InstrumentControls
    {
        public Slider VolumeSlider;
        public Image VolumeIcon;
        public Button VolumeButton;
        public Button RemoveButton;
    }

    private const string PianoInstrument = "Piano";
    private const string HiHatInstrument = "Hi-Hat";
    private const string SnareInstrument = "Snare";

    [Header("Tempo")]
    [SerializeField] private float bpm = 90f;
    [SerializeField] private int stepsPerBeat = 4;
    [SerializeField] private int loopSteps = 64;

    [Header("Piano Only")]
    [SerializeField] private PianoNoteMap pianoMap;

    [Header("Drum Clips")]
    [SerializeField] private AudioClip hiHatClip;
    [SerializeField] private int hiHatBaseRow;
    [SerializeField] private AudioClip snareClip;
    [SerializeField] private int snareBaseRow;

    [Header("Scheduling")]
    [SerializeField] private double scheduleAheadTime = 0.15;
    [SerializeField] private int poolSize = 24;

    [Header("Bottom Panel Icons")]
    [SerializeField] private Sprite volumeIconSprite;
    [SerializeField] private Sprite muteIconSprite;

    public List<NoteEvent> events => GetEventsForInstrument(_activeInstrument);

    public bool IsPlaying { get; private set; }
    public double SecPerStep => 60.0 / bpm / stepsPerBeat;
    public int LoopSteps => loopSteps;
    // The grid and note blocks read this so the editor reflects the selected loop
    public Color ActiveInstrumentColor => GetInstrumentColor(_activeInstrument);

    private readonly List<AudioSource> _pool = new();
    private readonly Dictionary<string, InstrumentControls> _instrumentControls = new();
    private readonly Dictionary<string, RectTransform> _instrumentRows = new();
    private readonly Dictionary<string, List<NoteEvent>> _instrumentEvents = new();
    private readonly Dictionary<string, float> _instrumentVolumes = new();
    private readonly Dictionary<string, bool> _instrumentMuted = new();
    private readonly List<string> _instrumentCreationOrder = new();
    private readonly List<string> _instrumentRegistrationOrder = new();
    private int _poolIndex;
    private string _activeInstrument = PianoInstrument;

    private PianoRollGrid _grid;
    private Image _rollHeaderBar;
    private TMP_Text _instrumentLabel;
    private Color _defaultRollHeaderColor;
    private UndoManager _undo;

    private double _dspStart;
    private double _nextScheduleDsp;
    private int _nextScheduleStep;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Keep the scene usable even if the serialized clip reference gets cleared in the editor
        if (hiHatClip == null)
            hiHatClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Hihat.wav");

        if (snareClip == null)
            snareClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Snare (G#).wav");
    }
#endif

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"AudioSource_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            _pool.Add(src);
        }

        _grid = FindFirstObjectByType<PianoRollGrid>();
        _undo = FindFirstObjectByType<UndoManager>();
        _rollHeaderBar = GameObject.Find("RollHeaderBar")?.GetComponent<Image>();
        _instrumentLabel = GameObject.Find("InstrumentLabel")?.GetComponent<TMP_Text>();
        _defaultRollHeaderColor = _rollHeaderBar != null
            ? _rollHeaderBar.color
            : new Color(0.18039216f, 0.23529412f, 0.36078432f, 1f);

        EnsureInstrumentEvents(PianoInstrument);
        EnsureInstrumentEvents(HiHatInstrument);
        EnsureInstrumentEvents(SnareInstrument);
        _instrumentVolumes[PianoInstrument] = 1f;
        _instrumentVolumes[HiHatInstrument] = 1f;
        _instrumentVolumes[SnareInstrument] = 1f;
        _instrumentMuted[PianoInstrument] = false;
        _instrumentMuted[HiHatInstrument] = false;
        _instrumentMuted[SnareInstrument] = false;

        // Register the right side buttons and bottom rows for each instrument loop
        InitializeInstrumentLoopsUi();
        SetActiveInstrument(PianoInstrument);
    }

    void Update()
    {
        if (!IsPlaying) return;

        double dspNow = AudioSettings.dspTime;

        while (_nextScheduleDsp < dspNow + scheduleAheadTime)
        {
            ScheduleStep(_nextScheduleStep, _nextScheduleDsp);

            _nextScheduleStep = (_nextScheduleStep + 1) % loopSteps;
            _nextScheduleDsp += SecPerStep;
        }
    }

    public void Play()
    {
        if (IsPlaying) return;
        IsPlaying = true;

        _dspStart = AudioSettings.dspTime + 0.05;
        _nextScheduleDsp = _dspStart;
        _nextScheduleStep = 0;
    }

    public void Stop()
    {
        IsPlaying = false;
        foreach (var src in _pool) src.Stop();
    }

    public void PreviewNoteRow(int row)
    {
        PreviewInstrumentNote(_activeInstrument, row);
    }

    public double GetLoopTime01()
    {
        if (!IsPlaying) return 0;
        double loopDur = loopSteps * SecPerStep;
        double t = (AudioSettings.dspTime - _dspStart) % loopDur;
        if (t < 0) t += loopDur;
        return t / loopDur;
    }

    private void ScheduleStep(int step, double dspTime)
    {
        if (pianoMap == null) return;

        // Each instrument keeps its own loop data and playback settings
        foreach (var pair in _instrumentEvents)
        {
            string instrumentName = pair.Key;
            var instrumentEvents = pair.Value;
            float instrumentVolume = GetInstrumentVolume(instrumentName);

            if (instrumentVolume <= 0f)
                continue;

            for (int i = 0; i < instrumentEvents.Count; i++)
            {
                var e = instrumentEvents[i];
                if (e.startStep != step) continue;

                if (!TryGetPlaybackSettings(instrumentName, e.row, out var clip, out var pitch))
                    continue;

                var src = GetPooledSource();
                src.clip = clip;
                src.volume = pianoMap.volume * instrumentVolume;
                src.pitch = pitch;
                src.PlayScheduled(dspTime);
            }
        }
    }

    private void PreviewInstrumentNote(string instrumentName, int row)
    {
        float instrumentVolume = GetInstrumentVolume(instrumentName);
        if (instrumentVolume <= 0f)
            return;

        if (!TryGetPlaybackSettings(instrumentName, row, out var clip, out var pitch))
            return;

        var src = GetPooledSource();
        src.Stop();
        src.clip = clip;
        src.volume = pianoMap != null ? pianoMap.volume * instrumentVolume : instrumentVolume;
        src.pitch = pitch;
        src.Play();
    }

    private AudioSource GetPooledSource()
    {
        var src = _pool[_poolIndex];
        _poolIndex = (_poolIndex + 1) % _pool.Count;
        return src;
    }

    private bool TryGetPlaybackSettings(string instrumentName, int row, out AudioClip clip, out float pitch)
    {
        clip = null;
        pitch = 1f;

        // Drum instruments use one recorded sample and transpose it by semitones per row
        if (instrumentName == HiHatInstrument && hiHatClip != null)
        {
            clip = hiHatClip;
            pitch = Mathf.Pow(2f, (row - hiHatBaseRow) / 12f);
            return true;
        }

        // Do not silently fall back to piano for drums if their clip is missing
        if (instrumentName == HiHatInstrument)
            return false;

        if (instrumentName == SnareInstrument && snareClip != null)
        {
            clip = snareClip;
            pitch = Mathf.Pow(2f, (row - snareBaseRow) / 12f);
            return true;
        }

        if (instrumentName == SnareInstrument)
            return false;

        if (pianoMap == null)
            return false;

        clip = pianoMap.GetClipForRow(row);
        return clip != null;
    }

    // Button: jump to start of loop while playing
    public void LoopBack()
    {
        if (!IsPlaying)
            return;

        // restart timing from "now" (small offset so scheduling is safe)
        _dspStart = AudioSettings.dspTime + 0.03;
        _nextScheduleDsp = _dspStart;
        _nextScheduleStep = 0;

        // stop currently playing pooled sources to avoid overlap
        foreach (var src in _pool)
            src.Stop();
    }

    public double GetLoopTimeSeconds()
    {
        if (!IsPlaying)
            return 0;

        double loopDuration = loopSteps * SecPerStep;

        double t = (AudioSettings.dspTime - _dspStart) % loopDuration;
        if (t < 0)
                t += loopDuration;

        return t;
    }

    private void InitializeInstrumentLoopsUi()
    {
        BindInstrumentLoop(PianoInstrument, "SoundItem_Piano", "Piano", showOnStart: true);
        BindInstrumentLoop(HiHatInstrument, "SoundItem_HiHat", "Hi-Hat", showOnStart: false);
        BindInstrumentLoop(SnareInstrument, "SoundItem_Snare", "Snare", showOnStart: false);
        RefreshInstrumentLoopLayout();
    }

    private void BindInstrumentLoop(string instrumentName, string buttonObjectName, string rowObjectName, bool showOnStart)
    {
        var buttonObject = GameObject.Find(buttonObjectName);
        var rowObject = GameObject.Find(rowObjectName);

        if (buttonObject == null || rowObject == null)
            return;

        var rowRect = rowObject.GetComponent<RectTransform>();
        if (rowRect == null)
            return;

        if (!_instrumentRegistrationOrder.Contains(instrumentName))
            _instrumentRegistrationOrder.Add(instrumentName);

        _instrumentRows[instrumentName] = rowRect;
        rowRect.gameObject.SetActive(showOnStart);
        // Bottom rows are also clickable so the user can switch the active loop there
        EnsureClickableRow(rowRect, instrumentName);
        BindInstrumentPanelControls(instrumentName, rowRect);

        var button = buttonObject.GetComponent<Button>();
        if (button == null)
            return;

        button.onClick.AddListener(() => OpenInstrumentLoop(instrumentName));
    }

    private void EnsureClickableRow(RectTransform rowRect, string instrumentName)
    {
        var image = rowRect.GetComponent<Image>();
        if (image == null)
            image = rowRect.gameObject.AddComponent<Image>();

        image.color = new Color(1f, 1f, 1f, 0.003921569f);
        image.raycastTarget = true;

        var button = rowRect.GetComponent<Button>();
        if (button == null)
            button = rowRect.gameObject.AddComponent<Button>();

        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SetActiveInstrument(instrumentName));
    }

    private void BindInstrumentPanelControls(string instrumentName, RectTransform rowRect)
    {
        var panel = FindDescendantByName(rowRect, "InstrumentPanel");
        if (panel == null)
            return;

        var controls = new InstrumentControls
        {
            VolumeSlider = FindDescendantByName(panel, "VolSlider")?.GetComponent<Slider>(),
            VolumeIcon = FindDescendantByName(panel, "VolIcon")?.GetComponent<Image>(),
            RemoveButton = FindDescendantByName(panel, "RemoveBtn")?.GetComponent<Button>()
        };

        if (controls.VolumeIcon != null)
        {
            controls.VolumeButton = controls.VolumeIcon.GetComponent<Button>();
            if (controls.VolumeButton == null)
                controls.VolumeButton = controls.VolumeIcon.gameObject.AddComponent<Button>();

            // The icon doubles as a mute toggle and swaps sprites when muted
            controls.VolumeButton.targetGraphic = controls.VolumeIcon;
            controls.VolumeButton.onClick.RemoveAllListeners();
            controls.VolumeButton.onClick.AddListener(() => ToggleInstrumentMute(instrumentName));
        }

        if (controls.VolumeSlider != null)
        {
            float startingVolume = controls.VolumeSlider.value <= 0f ? 1f : controls.VolumeSlider.value;
            _instrumentVolumes[instrumentName] = startingVolume;
            controls.VolumeSlider.SetValueWithoutNotify(startingVolume);
            controls.VolumeSlider.onValueChanged.RemoveAllListeners();
            controls.VolumeSlider.onValueChanged.AddListener(value => SetInstrumentVolume(instrumentName, value));
        }

        if (controls.RemoveButton != null)
        {
            controls.RemoveButton.onClick.RemoveAllListeners();
            controls.RemoveButton.onClick.AddListener(() => RemoveInstrumentPanel(instrumentName));
        }

        _instrumentControls[instrumentName] = controls;
        UpdateInstrumentVolumeIcon(instrumentName);
    }

    private void SetActiveInstrument(string instrumentName)
    {
        if (!_instrumentRows.TryGetValue(instrumentName, out var rowRect) || rowRect == null)
            return;

        if (!rowRect.gameObject.activeSelf)
            rowRect.gameObject.SetActive(true);

        // Switching instruments swaps the visible loop in the main piano roll editor
        _activeInstrument = instrumentName;
        UpdateInstrumentChrome(instrumentName);
        RefreshInstrumentLoopLayout();
        _grid?.RebuildAllViews();
    }

    private void OpenInstrumentLoop(string instrumentName)
    {
        RegisterInstrumentCreation(instrumentName);
        SetActiveInstrument(instrumentName);
    }

    private void SetInstrumentVolume(string instrumentName, float value)
    {
        _instrumentVolumes[instrumentName] = Mathf.Clamp01(value);
        UpdateInstrumentVolumeIcon(instrumentName);
    }

    private void ToggleInstrumentMute(string instrumentName)
    {
        bool isMuted = false;
        _instrumentMuted.TryGetValue(instrumentName, out isMuted);
        _instrumentMuted[instrumentName] = !isMuted;
        UpdateInstrumentVolumeIcon(instrumentName);
    }

    private void RemoveInstrumentPanel(string instrumentName)
    {
        // Deleting a panel is undoable and also clears that instrument's loop data
        _undo?.RecordInstrumentDelete(CaptureInstrumentState(instrumentName));

        if (_instrumentRows.TryGetValue(instrumentName, out var rowRect) && rowRect != null)
            rowRect.gameObject.SetActive(false);

        GetEventsForInstrument(instrumentName).Clear();

        if (_activeInstrument == instrumentName)
            SwitchToFirstVisibleInstrument();
        else
            RefreshInstrumentLoopLayout();
    }

    public InstrumentState CaptureInstrumentState(string instrumentName)
    {
        var state = new InstrumentState
        {
            InstrumentName = instrumentName,
            Volume = _instrumentVolumes.TryGetValue(instrumentName, out var volume) ? volume : 1f,
            Muted = _instrumentMuted.TryGetValue(instrumentName, out var muted) && muted,
            WasActiveInstrument = _activeInstrument == instrumentName,
            WasVisible = _instrumentRows.TryGetValue(instrumentName, out var rowRect) &&
                         rowRect != null &&
                         rowRect.gameObject.activeSelf
        };

        var notes = GetEventsForInstrument(instrumentName);
        for (int i = 0; i < notes.Count; i++)
            state.Notes.Add(CloneNote(notes[i]));

        return state;
    }

    public void RestoreInstrumentState(InstrumentState state)
    {
        if (state == null || string.IsNullOrEmpty(state.InstrumentName))
            return;

        var notes = GetEventsForInstrument(state.InstrumentName);
        notes.Clear();

        for (int i = 0; i < state.Notes.Count; i++)
            notes.Add(CloneNote(state.Notes[i]));

        _instrumentVolumes[state.InstrumentName] = state.Volume;
        _instrumentMuted[state.InstrumentName] = state.Muted;

        if (_instrumentRows.TryGetValue(state.InstrumentName, out var rowRect) && rowRect != null)
            rowRect.gameObject.SetActive(state.WasVisible);

        if (_instrumentControls.TryGetValue(state.InstrumentName, out var controls) && controls != null)
        {
            if (controls.VolumeSlider != null)
                controls.VolumeSlider.SetValueWithoutNotify(state.Volume);

            UpdateInstrumentVolumeIcon(state.InstrumentName);
        }

        if (state.WasActiveInstrument || _activeInstrument == state.InstrumentName)
            SetActiveInstrument(state.InstrumentName);
        else
        {
            RefreshInstrumentLoopLayout();
            _grid?.RebuildAllViews();
        }
    }

    private void SwitchToFirstVisibleInstrument()
    {
        if (TryActivateVisibleInstrument(PianoInstrument)) return;
        if (TryActivateVisibleInstrument(HiHatInstrument)) return;
        if (TryActivateVisibleInstrument(SnareInstrument)) return;

        _grid?.RebuildAllViews();
        RefreshInstrumentLoopLayout();
    }

    private bool TryActivateVisibleInstrument(string instrumentName)
    {
        return _instrumentRows.TryGetValue(instrumentName, out var rowRect) &&
               rowRect != null &&
               rowRect.gameObject.activeSelf &&
               ActivateVisibleInstrument(instrumentName);
    }

    private bool ActivateVisibleInstrument(string instrumentName)
    {
        SetActiveInstrument(instrumentName);
        return true;
    }

    private void UpdateInstrumentChrome(string instrumentName)
    {
        // The editor header reflects whichever loop is currently active
        if (_instrumentLabel != null)
            _instrumentLabel.text = instrumentName;

        if (_rollHeaderBar == null)
            return;

        _rollHeaderBar.color = GetInstrumentColor(instrumentName);
    }

    private Color GetInstrumentColor(string instrumentName)
    {
        return instrumentName switch
        {
            HiHatInstrument => new Color32(0x82, 0xA8, 0x66, 0xFF),
            SnareInstrument => new Color32(0xDB, 0x9B, 0x5E, 0xFF),
            _ => _defaultRollHeaderColor
        };
    }

    private List<NoteEvent> EnsureInstrumentEvents(string instrumentName)
    {
        if (!_instrumentEvents.TryGetValue(instrumentName, out var instrumentEvents))
        {
            instrumentEvents = new List<NoteEvent>();
            _instrumentEvents[instrumentName] = instrumentEvents;
        }

        return instrumentEvents;
    }

    private List<NoteEvent> GetEventsForInstrument(string instrumentName)
    {
        return EnsureInstrumentEvents(instrumentName);
    }

    private float GetInstrumentVolume(string instrumentName)
    {
        bool muted = false;
        if (_instrumentMuted.TryGetValue(instrumentName, out muted) && muted)
            return 0f;

        float volume = 1f;
        return _instrumentVolumes.TryGetValue(instrumentName, out volume) ? volume : 1f;
    }

    private void UpdateInstrumentVolumeIcon(string instrumentName)
    {
        if (!_instrumentControls.TryGetValue(instrumentName, out var controls) || controls.VolumeIcon == null)
            return;

        bool isMuted = false;
        _instrumentMuted.TryGetValue(instrumentName, out isMuted);

        controls.VolumeIcon.sprite = isMuted && muteIconSprite != null
            ? muteIconSprite
            : volumeIconSprite != null
                ? volumeIconSprite
                : controls.VolumeIcon.sprite;
    }

    private void RegisterInstrumentCreation(string instrumentName)
    {
        if (string.IsNullOrEmpty(instrumentName))
            return;

        if (!_instrumentCreationOrder.Contains(instrumentName))
            _instrumentCreationOrder.Add(instrumentName);

        ApplyInstrumentLoopOrder();
    }

    private void ApplyInstrumentLoopOrder()
    {
        int siblingIndex = 0;

        // Visible default rows stay in their base order, and newly created loops are appended after them
        for (int i = 0; i < _instrumentRegistrationOrder.Count; i++)
        {
            var instrumentName = _instrumentRegistrationOrder[i];
            if (_instrumentCreationOrder.Contains(instrumentName) ||
                !_instrumentRows.TryGetValue(instrumentName, out var rowRect) ||
                rowRect == null ||
                !rowRect.gameObject.activeSelf)
            {
                continue;
            }

            rowRect.SetSiblingIndex(siblingIndex);
            siblingIndex++;
        }

        // New loops keep their creation order, so the newest created loop ends up at the bottom
        for (int i = 0; i < _instrumentCreationOrder.Count; i++)
        {
            var instrumentName = _instrumentCreationOrder[i];
            if (!_instrumentRows.TryGetValue(instrumentName, out var rowRect) ||
                rowRect == null ||
                !rowRect.gameObject.activeSelf)
            {
                continue;
            }

            rowRect.SetSiblingIndex(siblingIndex);
            siblingIndex++;
        }

        ApplyInstrumentRowPositions();
    }

    private void ApplyInstrumentRowPositions()
    {
        int visibleIndex = 0;

        for (int i = 0; i < _instrumentCreationOrder.Count; i++)
        {
            if (TryApplyInstrumentRowPosition(_instrumentCreationOrder[i], visibleIndex))
                visibleIndex++;
        }

        for (int i = 0; i < _instrumentRegistrationOrder.Count; i++)
        {
            var instrumentName = _instrumentRegistrationOrder[i];
            if (_instrumentCreationOrder.Contains(instrumentName))
                continue;

            if (TryApplyInstrumentRowPosition(instrumentName, visibleIndex))
                visibleIndex++;
        }
    }

    private bool TryApplyInstrumentRowPosition(string instrumentName, int visibleIndex)
    {
        if (!_instrumentRows.TryGetValue(instrumentName, out var rowRect) ||
            rowRect == null ||
            !rowRect.gameObject.activeSelf)
        {
            return false;
        }

        float topOffset = 0f;
        var parentRect = rowRect.parent as RectTransform;
        if (parentRect != null)
            topOffset = parentRect.rect.height * (1f - parentRect.pivot.y);

        float cumulativeHeight = 0f;
        for (int i = 0; i < visibleIndex; i++)
        {
            if (TryGetVisibleRowByOrder(i, out var previousRow))
                cumulativeHeight += previousRow.rect.height;
        }

        rowRect.anchoredPosition = new Vector2(
            rowRect.anchoredPosition.x,
            topOffset - cumulativeHeight - (rowRect.rect.height * rowRect.pivot.y) - topOffset);

        return true;
    }

    private bool TryGetVisibleRowByOrder(int visibleIndex, out RectTransform rowRect)
    {
        rowRect = null;
        int currentIndex = 0;

        for (int i = 0; i < _instrumentRegistrationOrder.Count; i++)
        {
            var instrumentName = _instrumentRegistrationOrder[i];
            if (_instrumentCreationOrder.Contains(instrumentName))
                continue;

            if (!_instrumentRows.TryGetValue(instrumentName, out var candidate) ||
                candidate == null ||
                !candidate.gameObject.activeSelf)
            {
                continue;
            }

            if (currentIndex == visibleIndex)
            {
                rowRect = candidate;
                return true;
            }

            currentIndex++;
        }

        for (int i = 0; i < _instrumentCreationOrder.Count; i++)
        {
            var instrumentName = _instrumentCreationOrder[i];
            if (!_instrumentRows.TryGetValue(instrumentName, out var candidate) ||
                candidate == null ||
                !candidate.gameObject.activeSelf)
            {
                continue;
            }

            if (currentIndex == visibleIndex)
            {
                rowRect = candidate;
                return true;
            }

            currentIndex++;
        }

        return false;
    }

    private static Transform FindDescendantByName(Transform root, string objectName)
    {
        if (root.name == objectName)
            return root;

        for (int i = 0; i < root.childCount; i++)
        {
            var match = FindDescendantByName(root.GetChild(i), objectName);
            if (match != null)
                return match;
        }

        return null;
    }

    private static NoteEvent CloneNote(NoteEvent source)
    {
        return new NoteEvent
        {
            row = source.row,
            startStep = source.startStep,
            lengthSteps = source.lengthSteps
        };
    }

    private void RefreshInstrumentLoopLayout()
    {
        ApplyInstrumentLoopOrder();

        // Force Unity's layout system to notice rows being shown/hidden at runtime
        foreach (var rowRect in _instrumentRows.Values)
        {
            if (rowRect == null)
                continue;

            var current = rowRect;
            while (current != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(current);
                current = current.parent as RectTransform;
            }
        }

        Canvas.ForceUpdateCanvases();
    }
}
