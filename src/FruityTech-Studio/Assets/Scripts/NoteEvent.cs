using System;

[Serializable]
public class NoteEvent
{
    public string noteId;      
    public int stepIndex;       
    public int lengthSteps = 4; 
    public float velocity = 1f; 
}