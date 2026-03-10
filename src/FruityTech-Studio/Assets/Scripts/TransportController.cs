using UnityEngine;

public class TransportController : MonoBehaviour
{
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private TutorialManager tutorialManager;

    public void TogglePlay()
    {
        if (engine == null) return;

        bool wasPlaying = engine.IsPlaying;

        if (engine.IsPlaying) engine.Stop();
        else engine.Play();

        if (!wasPlaying)
            tutorialManager?.NotifyPlayPressed();
    }

    public void Stop()
    {
        if (engine == null) return;
        engine.Stop();
    }
}