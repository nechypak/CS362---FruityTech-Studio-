using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PianoNoteMap pianoMap;

    private void Awake()
    {
        if (slider != null && pianoMap != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = pianoMap.volume;

            slider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        if (pianoMap != null)
            pianoMap.volume = value;
    }
}