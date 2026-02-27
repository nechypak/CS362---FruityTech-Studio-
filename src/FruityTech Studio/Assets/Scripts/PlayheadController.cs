using UnityEngine;

public class PlayheadController : MonoBehaviour
{
    [Header("References")]
    public Sequencer sequencer;
    public RectTransform playhead;
    public RectTransform gridContent;
    private double _latencySec;

    [Header("Grid Settings")]
    public float cellWidth = 50f; 

    private void Awake()
    {
        int bufferLength, numBuffers;
        AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
        _latencySec = (double)(bufferLength * numBuffers) / AudioSettings.outputSampleRate;
    }

    private void Update()
    {
        if (sequencer == null || playhead == null || gridContent == null)
            return;

        // Высота playhead = высота всей сетки
        playhead.sizeDelta = new Vector2(
            playhead.sizeDelta.x,
            gridContent.sizeDelta.y
        );

        if (!sequencer.IsPlaying)
        {
            // Если не играет — возвращаем в начало
            playhead.anchoredPosition = new Vector2(0f, 0f);
            return;
        }

        double loopTime = sequencer.GetLoopTimeDsp() - _latencySec;
        if (loopTime < 0) loopTime += sequencer.SecPerStep * sequencer.loopSteps;

        // Сколько это шагов
        double stepFloat = loopTime / sequencer.SecPerStep;

        // Переводим шаги в пиксели
        float x = (float)(stepFloat * cellWidth);

        playhead.anchoredPosition = new Vector2(x, 0f);
    }
}