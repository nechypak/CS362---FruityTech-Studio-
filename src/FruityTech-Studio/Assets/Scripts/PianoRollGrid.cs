using UnityEngine;
using UnityEngine.EventSystems;

public class PianoRollGrid : MonoBehaviour, IPointerDownHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform notesLayer;   // GridBackground/NotesLayer
    [SerializeField] private RectTransform keyboardPanel;
    [SerializeField] private RectTransform pianoRollViewport;
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private UndoManager undo;           // optional (drag in scene)

    [Header("Grid")]
    [SerializeField] private int rows = 12;
    [SerializeField] private int steps = 64;
    [SerializeField] private float cellW = 32f;
    [SerializeField] private float cellH = 24f;

    [Header("Prefab")]
    [SerializeField] private NoteBlockView notePrefab;

    private RectTransform _gridRect;

    private void Awake()
    {
        _gridRect = (RectTransform)transform;
        SyncNotesLayerToGrid();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isActiveAndEnabled)
            return;

        SyncNotesLayerToGrid();
        RebuildAllViews();
    }

    // ---- Input ----

    public void OnPointerDown(PointerEventData eventData)
    {
        if (engine == null || notesLayer == null || notePrefab == null) return;

        if (!TryGetCellFromPointer(eventData, out int row, out int step))
            return;

        ToggleNote(row, step);
    }

    // ---- Public API ----

    public void RebuildAllViews()
    {
        SyncNotesLayerToGrid();
        ClearViews();
        foreach (var e in engine.events)
            SpawnView(e);
    }

    // ---- Core behavior ----

    private void ToggleNote(int row, int step)
    {
        // If there is a note occupying this cell -> remove it
        if (TryRemoveAt(row, step))
        {
            RebuildAllViews();
            return;
        }

        // Otherwise add a 1-step note
        var ev = new NoteEvent
        {
            row = row,
            startStep = step,
            lengthSteps = 1
        };

        if (WouldOverlap(ev))
            return;

        engine.events.Add(ev);
        undo?.RecordAdd(ev);
        // Only preview a newly placed note while stopped so playback stays clean
        if (!engine.IsPlaying)
            engine.PreviewNoteRow(row);

        // For 1-step notes we can just spawn; but rebuild keeps it consistent.
        SpawnView(ev);
    }

    private bool TryRemoveAt(int row, int step)
    {
        for (int i = engine.events.Count - 1; i >= 0; i--)
        {
            var e = engine.events[i];
            if (e.row != row) continue;

            if (step >= e.startStep && step < e.startStep + e.lengthSteps)
            {
                // snapshot for undo BEFORE removing
                var removed = new NoteEvent
                {
                    row = e.row,
                    startStep = e.startStep,
                    lengthSteps = e.lengthSteps
                };

                engine.events.RemoveAt(i);
                undo?.RecordRemove(removed);
                return true;
            }
        }
        return false;
    }

    private bool WouldOverlap(NoteEvent candidate)
    {
        int b0 = candidate.startStep;
        int b1 = candidate.startStep + candidate.lengthSteps;

        for (int i = 0; i < engine.events.Count; i++)
        {
            var e = engine.events[i];
            if (e.row != candidate.row) continue;

            int a0 = e.startStep;
            int a1 = e.startStep + e.lengthSteps;

            if (a0 < b1 && b0 < a1) // overlap
                return true;
        }
        return false;
    }

    // ---- UI helpers ----

    private bool TryGetCellFromPointer(PointerEventData eventData, out int row, out int step)
    {
        row = -1;
        step = -1;

        if (_gridRect == null)
            _gridRect = (RectTransform)transform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _gridRect,
                eventData.position,
                eventData.pressEventCamera,
                out var local))
            return false;

        var rect = _gridRect.rect;
        float gridHeight = GetGridHeight();
        float currentCellW = GetCellWidth();
        float currentCellH = GetCellHeight();

        float x = local.x - rect.xMin; // 0..width
        float yFromTop = rect.yMax - local.y; // 0...height from top edge

        if (yFromTop < 0f || yFromTop > gridHeight)
            return false;

        step = Mathf.FloorToInt(x / currentCellW);
        row  = Mathf.FloorToInt(yFromTop / currentCellH);

        if (step < 0 || step >= steps) return false;
        if (row  < 0 || row  >= rows)  return false;

        return true;
    }

    private void SpawnView(NoteEvent e)
    {
        var view = Instantiate(notePrefab, notesLayer);
        view.boundEvent = e;
        // Note blocks inherit the currently selected instrument's theme color
        view.ApplyColor(engine.ActiveInstrumentColor);

        var rt = (RectTransform)view.transform;
        float currentCellW = GetCellWidth();
        float currentCellH = GetCellHeight();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);

        rt.anchoredPosition = new Vector2(e.startStep * currentCellW, -(e.row * currentCellH));
        rt.sizeDelta = new Vector2(e.lengthSteps * currentCellW, currentCellH);
    }

    private void ClearViews()
    {
        for (int i = notesLayer.childCount - 1; i >= 0; i--)
            Destroy(notesLayer.GetChild(i).gameObject);
    }

    private float GetCellWidth()
    {
        return steps > 0 && _gridRect != null && _gridRect.rect.width > 0f
            ? _gridRect.rect.width / steps
            : cellW;
    }

    private float GetCellHeight()
    {
        return rows > 0 && GetGridHeight() > 0f
            ? GetGridHeight() / rows
            : cellH;
    }

    private float GetGridHeight()
    {
        if (pianoRollViewport != null && pianoRollViewport.rect.height > 0f)
            return pianoRollViewport.rect.height;

        if (keyboardPanel != null && keyboardPanel.rect.height > 0f)
            return keyboardPanel.rect.height;

        return _gridRect != null && _gridRect.rect.height > 0f
            ? _gridRect.rect.height
            : rows * cellH;
    }

    private void SyncNotesLayerToGrid()
    {
        if (notesLayer == null)
            return;

        if (_gridRect == null)
            _gridRect = (RectTransform)transform;

        var viewport = GetViewportRect();

        notesLayer.anchorMin = new Vector2(0f, 1f);
        notesLayer.anchorMax = new Vector2(0f, 1f);
        notesLayer.pivot = new Vector2(0f, 1f);
        notesLayer.anchoredPosition = Vector2.zero;
        notesLayer.sizeDelta = new Vector2(
            _gridRect.rect.width,
            GetGridHeight());
    }

    private RectTransform GetViewportRect()
    {
        if (pianoRollViewport != null)
            return pianoRollViewport;

        return _gridRect != null ? _gridRect : (RectTransform)transform;
    }
}
