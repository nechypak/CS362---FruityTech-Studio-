using NUnit.Framework;
using UnityEngine;
using System.Reflection;

// Tests interaction between UI control and engine playback
// Tests TransportController.cs and SequencerEngine.cs
public class TransportController_IntegrationTests
{
    [Test]
    public void TogglePlay_StartsAndStopsEngine()
    {
        var engineGO = new GameObject("Engine");
        var engine = engineGO.AddComponent<SequencerEngine>();

        var transportGO = new GameObject("Transport");
        var transport = transportGO.AddComponent<TransportController>();

        typeof(TransportController)
            .GetField("engine", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(transport, engine);

        // First toggle, then start playing
        transport.TogglePlay();
        Assert.IsTrue(engine.IsPlaying);

        // Second toggle, then stop playing
        transport.TogglePlay();
        Assert.IsFalse(engine.IsPlaying);

        Object.DestroyImmediate(engineGO);
        Object.DestroyImmediate(transportGO);
    }
}