using System.Collections.Generic;
using UnityEngine;

public class SequencerEngine : MonoBehaviour
{
    [Header("Tempo")]
    [SerializeField] private float bpm = 90f;
    [SerializeField] private int stepsPerBeat = 4;
    [SerializeField] private int loopSteps = 64;

    [Header("Piano Only")]
    [SerializeField] private PianoNoteMap pianoMap;

    [Header("Scheduling")]
    [SerializeField] private double scheduleAheadTime = 0.15;
    [SerializeField] private int poolSize = 24;

    public readonly List<NoteEvent> events = new();

    public bool IsPlaying { get; private set; }
    public double SecPerStep => 60.0 / bpm / stepsPerBeat;
    public int LoopSteps => loopSteps;

    private readonly List<AudioSource> _pool = new();
    private int _poolIndex;

    private double _dspStart;
    private double _nextScheduleDsp;
    private int _nextScheduleStep;

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"AudioSource_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            _pool.Add(src);
        }
    }

    void Update()
    {
        if (!IsPlaying) return;

        double dspNow = AudioSettings.dspTime;

        while (_nextScheduleDsp < dspNow + scheduleAheadTime)
        {
            ScheduleStep(_nextScheduleStep, _nextScheduleDsp);

            _nextScheduleStep = (_nextScheduleStep + 1) % loopSteps;
            _nextScheduleDsp += SecPerStep;
        }
    }

    public void Play()
    {
        if (IsPlaying) return;
        IsPlaying = true;

        _dspStart = AudioSettings.dspTime + 0.05;
        _nextScheduleDsp = _dspStart;
        _nextScheduleStep = 0;
    }

    public void Stop()
    {
        IsPlaying = false;
        foreach (var src in _pool) src.Stop();
    }

    public double GetLoopTime01()
    {
        if (!IsPlaying) return 0;
        double loopDur = loopSteps * SecPerStep;
        double t = (AudioSettings.dspTime - _dspStart) % loopDur;
        if (t < 0) t += loopDur;
        return t / loopDur;
    }

    private void ScheduleStep(int step, double dspTime)
    {
        if (pianoMap == null) return;

        for (int i = 0; i < events.Count; i++)
        {
            var e = events[i];
            if (e.startStep != step) continue;

            var clip = pianoMap.GetClipForRow(e.row);
            if (clip == null) continue;

            var src = GetPooledSource();
            src.clip = clip;
            src.volume = pianoMap.volume;
            src.PlayScheduled(dspTime);
        }
    }

    private AudioSource GetPooledSource()
    {
        var src = _pool[_poolIndex];
        _poolIndex = (_poolIndex + 1) % _pool.Count;
        return src;
    }

    // Button: jump to start of loop while playing
    public void LoopBack()
    {
        if (!IsPlaying)
            return;

        // restart timing from "now" (small offset so scheduling is safe)
        _dspStart = AudioSettings.dspTime + 0.03;
        _nextScheduleDsp = _dspStart;
        _nextScheduleStep = 0;

        // stop currently playing pooled sources to avoid overlap
        foreach (var src in _pool)
            src.Stop();
    }

    public double GetLoopTimeSeconds()
    {
        if (!IsPlaying)
            return 0;

        double loopDuration = loopSteps * SecPerStep;

        double t = (AudioSettings.dspTime - _dspStart) % loopDuration;
        if (t < 0)
                t += loopDuration;

        return t;
    }
}