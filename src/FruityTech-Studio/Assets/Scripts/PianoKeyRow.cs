using UnityEngine;
using UnityEngine.EventSystems;

public class PianoKeyRow : MonoBehaviour, IPointerDownHandler
{
    public string noteId;   
    public int semitone;    

    public void OnPointerDown(PointerEventData eventData)
    {
        var engine = FindFirstObjectByType<SequencerEngine>();
        if (engine == null)
            return;

        // Clicking the key list auditions the active instrument at this row.
        engine.PreviewNoteRow(semitone);
    }
}
