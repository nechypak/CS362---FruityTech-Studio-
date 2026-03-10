using UnityEngine;

[CreateAssetMenu(menuName = "FruityTech/Piano Note Map")]
public class PianoNoteMap : ScriptableObject
{
    [Tooltip("Row 0..11 (LowC..LowB)")]
    public AudioClip[] rowClips = new AudioClip[12];

    [Range(0f, 1f)] public float volume = 0.8f;

    public AudioClip GetClipForRow(int row)
    {
        if (rowClips == null || rowClips.Length == 0) return null;
        row = Mathf.Clamp(row, 0, rowClips.Length - 1);
        return rowClips[row];
    }
}
