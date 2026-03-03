using TMPro;
using UnityEngine;

public class TimeTextController : MonoBehaviour
{
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private TMP_Text timeText;

    void Update()
    {
        if (engine == null || timeText == null)
            return;

        if (!engine.IsPlaying)
        {
            timeText.text = "00:00.00";
            return;
        }

        double t = engine.GetLoopTimeSeconds();

        int minutes = (int)(t / 60.0);
        double seconds = t - (minutes * 60.0);

        timeText.text = $"{minutes:00}:{seconds:00.00}";
    }
}