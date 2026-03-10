using UnityEngine;

public class TransportController : MonoBehaviour
{
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject stopButton;

    private bool _lastPlaying;

    private void Awake()
    {
        if (playButton == null)
            playButton = GameObject.Find("BtnPlay");

        if (stopButton == null)
            stopButton = GameObject.Find("BtnStop");

        SyncTransportButtons(force: true);
    }

    private void Update()
    {
        if (engine == null) return;

        if (engine.IsPlaying != _lastPlaying)
            SyncTransportButtons(force: false);
    }

    public void TogglePlay()
    {
        if (engine == null) return;

        bool wasPlaying = engine.IsPlaying;

        if (engine.IsPlaying) engine.Stop();
        else engine.Play();

        if (!wasPlaying)
            tutorialManager?.NotifyPlayPressed();

        SyncTransportButtons(force: false);
    }

    public void Stop()
    {
        if (engine == null) return;
        engine.Stop();
        SyncTransportButtons(force: false);
    }

    private void SyncTransportButtons(bool force)
    {
        if (engine == null) return;

        if (!force && engine.IsPlaying == _lastPlaying)
            return;

        _lastPlaying = engine.IsPlaying;

        if (playButton != null)
            playButton.SetActive(!engine.IsPlaying);

        if (stopButton != null)
            stopButton.SetActive(engine.IsPlaying);
    }
}
