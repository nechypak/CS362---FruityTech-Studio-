using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    // InstrumentDelete restores a removed bottom instrument row plus its loop state
    public enum ActionType { Add, Remove, InstrumentDelete }

    public sealed class UndoAction
    {
        public ActionType type;
        public NoteEvent note; // snapshot
        public SequencerEngine.InstrumentState instrumentState;
    }

    [SerializeField] private SequencerEngine engine;
    [SerializeField] private PianoRollGrid grid; // so we can rebuild visuals after undo

    private readonly Stack<UndoAction> _stack = new();

    public void RecordAdd(NoteEvent n)
    {
        _stack.Push(new UndoAction { type = ActionType.Add, note = Clone(n) });
    }

    public void RecordRemove(NoteEvent n)
    {
        _stack.Push(new UndoAction { type = ActionType.Remove, note = Clone(n) });
    }

    public void RecordInstrumentDelete(SequencerEngine.InstrumentState state)
    {
        if (state == null)
            return;

        _stack.Push(new UndoAction
        {
            type = ActionType.InstrumentDelete,
            instrumentState = state
        });
    }

    public void Undo()
    {
        if (_stack.Count == 0 || engine == null) return;

        var act = _stack.Pop();

        if (act.type == ActionType.InstrumentDelete)
        {
            // Restoring an instrument also restores its notes, volume, mute, and visibility
            engine.RestoreInstrumentState(act.instrumentState);
        }
        else if (act.type == ActionType.Add)
        {
            // Undo an Add => remove that note
            RemoveMatching(engine, act.note);
        }
        else
        {
            // Undo a Remove => add it back (if no overlap)
            if (!WouldOverlap(engine, act.note))
                engine.events.Add(Clone(act.note));
        }

        if (grid != null)
            grid.RebuildAllViews(); // refresh UI blocks
    }

    private static NoteEvent Clone(NoteEvent n)
        => new NoteEvent { row = n.row, startStep = n.startStep, lengthSteps = n.lengthSteps };

    private static void RemoveMatching(SequencerEngine eng, NoteEvent target)
    {
        for (int i = eng.events.Count - 1; i >= 0; i--)
        {
            var e = eng.events[i];
            if (e.row == target.row &&
                e.startStep == target.startStep &&
                e.lengthSteps == target.lengthSteps)
            {
                eng.events.RemoveAt(i);
                return;
            }
        }
    }

    private static bool WouldOverlap(SequencerEngine eng, NoteEvent n)
    {
        for (int i = 0; i < eng.events.Count; i++)
        {
            var e = eng.events[i];
            if (e.row != n.row) continue;

            int a0 = e.startStep;
            int a1 = e.startStep + e.lengthSteps;
            int b0 = n.startStep;
            int b1 = n.startStep + n.lengthSteps;

            if (a0 < b1 && b0 < a1) return true;
        }
        return false;
    }
}
