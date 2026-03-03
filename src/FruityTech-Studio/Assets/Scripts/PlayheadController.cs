using UnityEngine;

public class PlayheadController : MonoBehaviour
{
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private RectTransform playhead;
    [SerializeField] private float loopWidthPx = 2048f;

    void Update()
    {
        if (!engine || !playhead) return;

        float t01 = (float)engine.GetLoopTime01();
        var pos = playhead.anchoredPosition;
        pos.x = t01 * loopWidthPx;
        playhead.anchoredPosition = pos;
    }
}