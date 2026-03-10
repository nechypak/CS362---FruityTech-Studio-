using System;
using System.Collections.Generic;

[Serializable]
public class SongSaveData
{
    public string songTitle;
    public float bpm;
    public List<NoteEvent> notes = new();
}