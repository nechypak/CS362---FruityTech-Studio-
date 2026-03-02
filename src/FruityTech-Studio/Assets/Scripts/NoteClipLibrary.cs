using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NoteClip
{
    public string noteId;   
    public AudioClip clip;
}

public class NoteClipLibrary : MonoBehaviour
{
    public NoteClip[] clips;

    private Dictionary<string, AudioClip> _map;

    private void Awake()
    {
        _map = new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);

        foreach (var nc in clips)
        {
            if (string.IsNullOrWhiteSpace(nc.noteId) || nc.clip == null) continue;
            string key = nc.noteId.Trim();
            if (!_map.ContainsKey(key))
                _map.Add(key, nc.clip);
        }
    }

    public AudioClip Get(string noteId)
    {
        if (noteId == null) return null;
        _map.TryGetValue(noteId.Trim(), out var clip);
        return clip;
    }
}