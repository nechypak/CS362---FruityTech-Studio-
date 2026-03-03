using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SequencerEngine_SystemTests
{
    [UnityTest]
    public IEnumerator Awake_CreatesAudioPool_AndLoopBackResetsTime()
    {
        var engineGO = new GameObject("Engine");
        var engine = engineGO.AddComponent<SequencerEngine>();

        yield return null; // Awake should have run and created pooled AudioSources as children

        // Default poolSize in script is 24. This asserts your runtime environment behavior.
        Assert.AreEqual(24, engine.transform.childCount);

        engine.Play();
        Assert.IsTrue(engine.IsPlaying);

        // Let time progress beyond the dspStart offset
        yield return new WaitForSeconds(0.20f);

        double t1 = engine.GetLoopTimeSeconds();
        Assert.Greater(t1, 0.0);

        // Now system action: loop back
        engine.LoopBack();

        // Wait until after the 0.03s offset used by LoopBack
        yield return new WaitForSeconds(0.06f);

        double t2 = engine.GetLoopTimeSeconds();

        // After loop back, loop time should be near the start again (small)
        Assert.Less(t2, 0.10);

        engine.Stop();
        Assert.IsFalse(engine.IsPlaying);

        Object.Destroy(engineGO);
    }
}