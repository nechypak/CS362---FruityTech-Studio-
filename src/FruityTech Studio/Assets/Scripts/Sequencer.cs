using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    [Header("Fixed tempo")]
    [SerializeField] private float bpm = 90f;
    [SerializeField] private int stepsPerBeat = 4;

    [Header("Loop")]
    public int loopSteps = 64;

    [Header("Audio")]
    public NoteClipLibrary library;
    [Range(0f, 1f)] public float masterVolume = 0.8f;

    [Header("Scheduling")]
    [Tooltip("How far ahead (in seconds) we schedule audio events.")]
    public double scheduleAheadTime = 0.15;

    [Header("Sequence data")]
    public List<NoteEvent> events = new List<NoteEvent>();

    public bool IsPlaying => _isPlaying;
    public float Bpm => bpm;
    public int StepsPerBeat => stepsPerBeat;
    public double SecPerStep => _secPerStep;

    private bool _isPlaying;

    private double _secPerStep;
    private double _loopDuration;

    private double[] _sourceFreeDspTime;
    private double _startDspTime;      // fixed start time (do NOT change while playing)
    private long _globalStep;          // counts steps forever (prevents drift)
    private double _nextStepDspTime;   // dsp time for next scheduled step

    private readonly List<AudioSource> _pool = new List<AudioSource>();
    private int _poolIndex = 0;

    [Header("AudioSource Pool")]
    [SerializeField] private int poolSize = 24;

    private void Awake()
    {
        RecomputeTiming();

        // Create pool
        for (int i = 0; i < poolSize; i++)
        {
            var a = gameObject.AddComponent<AudioSource>();
            a.playOnAwake = false;
            _pool.Add(a);
        }

        _sourceFreeDspTime = new double[_pool.Count];
        for (int i = 0; i < _sourceFreeDspTime.Length; i++)
            _sourceFreeDspTime[i] = 0.0;
    }

    private void OnValidate()
    {
        if (stepsPerBeat < 1) stepsPerBeat = 1;
        if (bpm < 1f) bpm = 1f;
        if (loopSteps < 1) loopSteps = 1;

        // In edit mode this keeps values coherent
        RecomputeTiming();
    }

    private void RecomputeTiming()
    {
        _secPerStep = (60.0 / bpm) / stepsPerBeat;
        _loopDuration = loopSteps * _secPerStep;
    }

    public void Play()
    {
        if (library == null) return;

        RecomputeTiming();

        _isPlaying = true;

        // Start slightly in the future so first scheduled events are not late.
        _startDspTime = AudioSettings.dspTime + 0.05;

        _globalStep = 0;
        _nextStepDspTime = _startDspTime;
    }

    public void Stop()
    {
        _isPlaying = false;

        for (int i = 0; i < _pool.Count; i++)
            _pool[i].Stop();
    }

    public void TogglePlay()
    {
        if (_isPlaying) Stop();
        else Play();
    }

    private void Update()
    {
        if (!_isPlaying || library == null) return;

        double dspNow = AudioSettings.dspTime;

        // Schedule steps ahead
        while (_nextStepDspTime < dspNow + scheduleAheadTime)
        {
            int stepInLoop = (int)(_globalStep % loopSteps);

            ScheduleStep(stepInLoop, _nextStepDspTime);

            _globalStep++;
            _nextStepDspTime = _startDspTime + _globalStep * _secPerStep;
        }
    }

    private void ScheduleStep(int stepIndex, double dspTime)
    {
        for (int i = 0; i < events.Count; i++)
        {
            var e = events[i];
            if (e.stepIndex != stepIndex) continue;

            var clip = library.Get(e.noteId);
            if (clip == null) continue;

            var src = GetPooledSource(dspTime, clip.length);
            src.clip = clip;
            src.volume = Mathf.Clamp01(masterVolume * Mathf.Clamp01(e.velocity));
            src.PlayScheduled(dspTime);
        }
    }
private AudioSource GetPooledSource(double dspTimeNeeded, double clipLen)
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            int idx = (_poolIndex + i) % _pool.Count;
            if (_sourceFreeDspTime[idx] <= dspTimeNeeded)
            {
                _poolIndex = (idx + 1) % _pool.Count;
                _sourceFreeDspTime[idx] = dspTimeNeeded + clipLen;
                return _pool[idx];
            }
        }

        var a = gameObject.AddComponent<AudioSource>();
        a.playOnAwake = false;
        _pool.Add(a);

        var newArr = new double[_pool.Count];
        for (int i = 0; i < _sourceFreeDspTime.Length; i++) newArr[i] = _sourceFreeDspTime[i];
        newArr[newArr.Length - 1] = dspTimeNeeded + clipLen;
        _sourceFreeDspTime = newArr;

        _poolIndex = _pool.Count % _pool.Count;
        return a;
    }
    public double GetLoopTimeDsp()
    {
        if (!_isPlaying) return 0.0;

        double t = AudioSettings.dspTime - _startDspTime;
        if (t < 0) t = 0;

        return t % _loopDuration;
    }

    public double GetSongTimeDsp()
    {
        if (!_isPlaying) return 0.0;

        double t = AudioSettings.dspTime - _startDspTime;
        return t < 0 ? 0 : t;
    }
}