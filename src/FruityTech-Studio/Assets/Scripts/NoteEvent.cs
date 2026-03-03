using System;

[Serializable]
public class NoteEvent
{
    public int row;          // 0..11
    public int startStep;    // 0..loopSteps-1
    public int lengthSteps;  // >=1
}