using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class UndoManager_UnitTests
{
    // Tests undoManager.cs 
    [Test]
    public void Undo_ReversesLastRecordedAction_AddAndRemove()
    {
        // Engine with one note
        var engineGO = new GameObject("Engine");
        var engine = engineGO.AddComponent<SequencerEngine>();

        var undoGO = new GameObject("Undo");
        var undo = undoGO.AddComponent<UndoManager>();

        // Inject private [SerializeField] engine, and keep grid null (unit scope)
        typeof(UndoManager)
            .GetField("engine", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(undo, engine);

        // --- Case A: RecordAdd then Undo => removes that note ---
        var noteA = new NoteEvent { row = 2, startStep = 10, lengthSteps = 1 };
        engine.events.Add(noteA);

        undo.RecordAdd(noteA);
        Assert.AreEqual(1, engine.events.Count);

        undo.Undo();
        Assert.AreEqual(0, engine.events.Count);

        // --- Case B: RecordRemove then Undo => adds it back (no overlap) ---
        var noteB = new NoteEvent { row = 4, startStep = 12, lengthSteps = 2 };

        // simulate a remove happened earlier by recording the remove snapshot
        undo.RecordRemove(noteB);
        Assert.AreEqual(0, engine.events.Count);

        undo.Undo();
        Assert.AreEqual(1, engine.events.Count);
        Assert.AreEqual(4, engine.events[0].row);
        Assert.AreEqual(12, engine.events[0].startStep);
        Assert.AreEqual(2, engine.events[0].lengthSteps);

        Object.DestroyImmediate(undoGO);
        Object.DestroyImmediate(engineGO);
    }
}