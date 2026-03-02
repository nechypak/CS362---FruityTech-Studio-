using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Sequencer sequencer;

    public Button playStopButton;
    public TMP_Text playText;
    public TMP_Text bpmLabel;
    public Slider volumeSlider;

    private void Start()
    {
        bpmLabel.text = "90 BPM"; 

        playStopButton.onClick.AddListener(() =>
        {
            sequencer.TogglePlay();
            playText.text = sequencer.IsPlaying ? "Stop" : "Play";
        });

        volumeSlider.onValueChanged.AddListener(v =>
        {
            sequencer.masterVolume = Mathf.Clamp01(v);
        });

        playText.text = "Play";
    }
}