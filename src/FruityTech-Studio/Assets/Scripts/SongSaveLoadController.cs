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
    [SerializeField] private bool useEditorFileDialog = true;
    [SerializeField] private string saveFolderName = "Songs";

    public void SaveSong()
    {
        if (engine == null)
        {
            Debug.LogError("SongSaveLoadController: Engine is not assigned.");
            return;
        }

        SongSaveData data = BuildSaveData();

        string path = GetDefaultSavePath();

#if UNITY_EDITOR
        if (useEditorFileDialog)
        {
            string suggestedName = GetSafeFileName(GetSongTitleOrDefault());
            path = EditorUtility.SaveFilePanel(
                "Save Song",
                "",
                suggestedName,
                extension
            );
        }
#endif

        if (string.IsNullOrEmpty(path))
            return;

        EnsureSaveDirectoryExists(path);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"Song saved to: {path}");
    }

    public void LoadSong()
    {
        if (engine == null || pianoRollGrid == null)
        {
            Debug.LogError("SongSaveLoadController: Engine or PianoRollGrid is not assigned.");
            return;
        }

        string path = GetDefaultSavePath();

#if UNITY_EDITOR
        if (useEditorFileDialog)
        {
            path = EditorUtility.OpenFilePanel(
                "Load Song",
                "",
                extension
            );
        }
#endif

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

    private string GetDefaultSavePath()
    {
        string safeName = GetSafeFileName(GetSongTitleOrDefault());
        string ext = string.IsNullOrWhiteSpace(extension) ? "json" : extension.Trim();
        if (!ext.StartsWith("."))
            ext = "." + ext;

        string folder = Path.Combine(Application.persistentDataPath, saveFolderName);
        return Path.Combine(folder, safeName + ext);
    }

    private void EnsureSaveDirectoryExists(string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
}
