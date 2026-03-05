using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SequencerEngine_SystemTests
{
    // Tests the engines runtime behavior
    // Tests SequencerEngine.cs
    [UnityTest]
    public IEnumerator Awake_CreatesAudioPool_AndLoopBackResetsTime()
    {
        // Ensure there's an AudioListener in the scene (CI often has none)
        var listenerGO = new GameObject("Listener");
        listenerGO.AddComponent<AudioListener>();

        var engineGO = new GameObject("Engine");
        var engine = engineGO.AddComponent<SequencerEngine>();

        yield return null;

        Assert.AreEqual(24, engine.transform.childCount);

        engine.Play();
        Assert.IsTrue(engine.IsPlaying);

        // Let it run a bit
        yield return new WaitForSeconds(0.2f);

        double t1 = engine.GetLoopTimeSeconds();
        Assert.Greater(t1, 0.0);

        engine.LoopBack();

        // Wait up to ~1s for loop time to reset (robust in CI)
        float timeout = 1.0f;
        while (timeout > 0f)
        {
            double t2 = engine.GetLoopTimeSeconds();
            if (t2 < 0.10) break;

            timeout -= Time.deltaTime;
            yield return null;
        }

        Assert.Less(engine.GetLoopTimeSeconds(), 0.10);

        engine.Stop();
        Assert.IsFalse(engine.IsPlaying);

        Object.Destroy(engineGO);
        Object.Destroy(listenerGO);
    }
}