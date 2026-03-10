using UnityEngine;
using UnityEngine.EventSystems;

public class PianoRollGrid : MonoBehaviour, IPointerDownHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform notesLayer;
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private UndoManager undo;
    [SerializeField] private TutorialManager tutorialManager;

    [Header("Grid")]
    [SerializeField] private int rows = 12;
    [SerializeField] private int steps = 64;
    [SerializeField] private float cellW = 32f;
    [SerializeField] private float cellH = 24f;

    [Header("Prefab")]
    [SerializeField] private NoteBlockView notePrefab;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (engine == null || notesLayer == null || notePrefab == null) return;

        if (!TryGetCellFromPointer(eventData, out int row, out int step))
            return;

        ToggleNote(row, step);
    }

    public void RebuildAllViews()
    {
        ClearViews();
        foreach (var e in engine.events)
            SpawnView(e);
    }

    private void ToggleNote(int row, int step)
    {
        if (TryRemoveAt(row, step))
        {
            RebuildAllViews();
            return;
        }

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
        tutorialManager?.NotifyNotePlaced();

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

            if (a0 < b1 && b0 < a1)
                return true;
        }
        return false;
    }

    private bool TryGetCellFromPointer(PointerEventData eventData, out int row, out int step)
    {
        row = -1;
        step = -1;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform,
                eventData.position,
                eventData.pressEventCamera,
                out var local))
            return false;

        var rect = ((RectTransform)transform).rect;

        float x = local.x - rect.xMin;
        float y = local.y - rect.yMin;

        step = Mathf.FloorToInt(x / cellW);
        row = Mathf.FloorToInt(y / cellH);

        if (step < 0 || step >= steps) return false;
        if (row < 0 || row >= rows) return false;

        return true;
    }

    private void SpawnView(NoteEvent e)
    {
        var view = Instantiate(notePrefab, notesLayer);
        view.boundEvent = e;

        var rt = (RectTransform)view.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;

        rt.anchoredPosition = new Vector2(e.startStep * cellW, e.row * cellH);
        rt.sizeDelta = new Vector2(e.lengthSteps * cellW, cellH);
    }

    private void ClearViews()
    {
        for (int i = notesLayer.childCount - 1; i >= 0; i--)
            Destroy(notesLayer.GetChild(i).gameObject);
    }
}