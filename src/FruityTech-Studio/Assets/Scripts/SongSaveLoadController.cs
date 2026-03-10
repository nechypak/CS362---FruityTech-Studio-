using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SongSaveLoadController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SequencerEngine engine;
    [SerializeField] private PianoRollGrid pianoRollGrid;
    [SerializeField] private TMP_InputField songTitleInput;

    [Header("Options")]
    [SerializeField] private string defaultFileName = "MySong";
    [SerializeField] private string extension = "json";

    public void SaveSong()
    {
        if (engine == null)
        {
            Debug.LogError("SongSaveLoadController: Engine is not assigned.");
            return;
        }

#if UNITY_EDITOR
        SongSaveData data = BuildSaveData();

        string suggestedName = GetSafeFileName(GetSongTitleOrDefault());
        string path = EditorUtility.SaveFilePanel(
            "Save Song",
            "",
            suggestedName,
            extension
        );

        if (string.IsNullOrEmpty(path))
            return;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"Song saved to: {path}");
#else
        Debug.LogWarning("SaveSong uses Unity Editor native picker only in this version.");
#endif
    }

    public void LoadSong()
    {
        if (engine == null || pianoRollGrid == null)
        {
            Debug.LogError("SongSaveLoadController: Engine or PianoRollGrid is not assigned.");
            return;
        }

#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel(
            "Load Song",
            "",
            extension
        );

        if (string.IsNullOrEmpty(path))
            return;

        if (!File.Exists(path))
        {
            Debug.LogError($"Load failed. File does not exist: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        SongSaveData data = JsonUtility.FromJson<SongSaveData>(json);

        if (data == null)
        {
            Debug.LogError("Load failed. JSON could not be parsed.");
            return;
        }

        ApplyLoadedSong(data);

        Debug.Log($"Song loaded from: {path}");
#else
        Debug.LogWarning("LoadSong uses Unity Editor native picker only in this version.");
#endif
    }

    private SongSaveData BuildSaveData()
    {
        SongSaveData data = new SongSaveData
        {
            songTitle = GetSongTitleOrDefault(),
            bpm = engine.GetBpm(),
            notes = new List<NoteEvent>()
        };

        foreach (var e in engine.events)
        {
            data.notes.Add(new NoteEvent
            {
                row = e.row,
                startStep = e.startStep,
                lengthSteps = e.lengthSteps
            });
        }

        return data;
    }

    private void ApplyLoadedSong(SongSaveData data)
    {
        engine.Stop();
        engine.events.Clear();

        if (data.notes != null)
        {
            foreach (var e in data.notes)
            {
                if (e == null) continue;

                engine.events.Add(new NoteEvent
                {
                    row = e.row,
                    startStep = e.startStep,
                    lengthSteps = Mathf.Max(1, e.lengthSteps)
                });
            }
        }

        engine.SetBpm(data.bpm);

        if (songTitleInput != null)
            songTitleInput.text = string.IsNullOrWhiteSpace(data.songTitle) ? defaultFileName : data.songTitle;

        pianoRollGrid.RebuildAllViews();
    }

    private string GetSongTitleOrDefault()
    {
        if (songTitleInput == null || string.IsNullOrWhiteSpace(songTitleInput.text))
            return defaultFileName;

        return songTitleInput.text.Trim();
    }

    private string GetSafeFileName(string rawName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            rawName = rawName.Replace(c, '_');

        return string.IsNullOrWhiteSpace(rawName) ? defaultFileName : rawName;
    }
}