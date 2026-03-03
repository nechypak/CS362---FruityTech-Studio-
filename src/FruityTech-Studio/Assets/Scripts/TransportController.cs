using UnityEngine;

public class TransportController : MonoBehaviour
{
    [SerializeField] private SequencerEngine engine;

    public void TogglePlay()
    {
        if (engine == null) return;

        if (engine.IsPlaying) engine.Stop();
        else engine.Play();
    }

    public void Stop()
    {
        if (engine == null) return;
        engine.Stop();
    }
}