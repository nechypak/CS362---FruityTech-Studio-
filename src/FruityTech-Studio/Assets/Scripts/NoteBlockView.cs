using UnityEngine;
using UnityEngine.UI;

public class NoteBlockView : MonoBehaviour
{
    public NoteEvent boundEvent;

    [SerializeField] private Image fillImage;

    private void Awake()
    {
        if (fillImage == null)
            fillImage = GetComponent<Image>();
    }

    public void ApplyColor(Color color)
    {
        if (fillImage == null)
            fillImage = GetComponent<Image>();

        // Note color follows the active instrument so the grid matches the header theme
        if (fillImage != null)
            fillImage.color = color;
    }
}
