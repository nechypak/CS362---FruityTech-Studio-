using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Transport_Playhead_IntegrationTests
{
    [UnityTest]
    public IEnumerator TogglePlay_StartsEngine_AndPlayheadMoves()
    {
        // Engine
        var engineGO = new GameObject("Engine");
        var engine = engineGO.AddComponent<SequencerEngine>();

        // Transport
        var transportGO = new GameObject("Transport");
        var transport = transportGO.AddComponent<TransportController>();
        typeof(TransportController)
            .GetField("engine", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(transport, engine);

        // Playhead
        var playheadRT = new GameObject("Playhead").AddComponent<RectTransform>();
        var playheadCtrlGO = new GameObject("PlayheadController");
        var playheadCtrl = playheadCtrlGO.AddComponent<PlayheadController>();

        typeof(PlayheadController)
            .GetField("engine", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(playheadCtrl, engine);

        typeof(PlayheadController)
            .GetField("playhead", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(playheadCtrl, playheadRT);

        typeof(PlayheadController)
            .GetField("loopWidthPx", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(playheadCtrl, 1000f);

        yield return null; // let Awake run

        Assert.IsFalse(engine.IsPlaying);

        // Toggle play ON
        transport.TogglePlay();
        Assert.IsTrue(engine.IsPlaying);

        float x0 = playheadRT.anchoredPosition.x;

        // Wait long enough to pass the engine's dspStart offset (0.05s)
        yield return new WaitForSeconds(0.12f);
        yield return null;

        float x1 = playheadRT.anchoredPosition.x;

        Assert.Greater(x1, x0);

        Object.Destroy(engineGO);
        Object.Destroy(transportGO);
        Object.Destroy(playheadCtrlGO);
        Object.Destroy(playheadRT.gameObject);
    }
}