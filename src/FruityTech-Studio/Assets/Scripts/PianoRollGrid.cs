using UnityEngine;
using UnityEngine.EventSystems;

public class PianoRollGrid : MonoBehaviour, IPointerDownHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform notesLayer;   // GridBackground/NotesLayer
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private UndoManager undo;           // optional (drag in scene)

    [Header("Grid")]
    [SerializeField] private int rows = 12;
    [SerializeField] private int steps = 64;
    [SerializeField] private float cellW = 32f;
    [SerializeField] private float cellH = 24f;

    [Header("Prefab")]
    [SerializeField] private NoteBlockView notePrefab;

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

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform,
                eventData.position,
                eventData.pressEventCamera,
                out var local))
            return false;

        var rect = ((RectTransform)transform).rect;

        float x = local.x - rect.xMin; // 0..width
        float y = local.y - rect.yMin; // 0..height

        step = Mathf.FloorToInt(x / cellW);
        row  = Mathf.FloorToInt(y / cellH);

        if (step < 0 || step >= steps) return false;
        if (row  < 0 || row  >= rows)  return false;

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