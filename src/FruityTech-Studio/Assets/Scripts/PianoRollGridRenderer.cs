using UnityEngine;
using UnityEngine.UI;

public class PianoRollGridRenderer : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public RectTransform gridBackground;
    public RectTransform keyboardPanel;
    public RectTransform pianoRollViewport;
    public Image linePrefab; // prefab with 1x1 sprite

    [Header("Sizing")]
    public int rows = 12;
    public float rowHeight = 43f;

    public int steps = 64;
    public float stepWidth = 32f;

    public int stepsPerBeat = 4;
    public int beatsPerBar = 4;

    [Header("Colors (RGBA)")]
    public Color thin = new Color(43/255f, 51/255f, 64/255f, 1f);      // #2B3340
    public Color beat = new Color(58/255f, 70/255f, 88/255f, 1f);      // #3A4658
    public Color bar  = new Color(90/255f, 110/255f, 140/255f, 1f);    // stronger

    [SerializeField] private UndoManager undo;
    [SerializeField] private RectTransform notesLayer;

    void Awake()
    {
        Build();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isActiveAndEnabled)
            return;

        Build();
    }

    [ContextMenu("Build Grid")]
    public void Build()
    {
        if (!gridBackground) gridBackground = GetComponent<RectTransform>();
        if (!linePrefab) { Debug.LogError("Assign linePrefab"); return; }

        // Clear old
        for (int i = gridBackground.childCount - 1; i >= 0; i--)
            DestroyImmediate(gridBackground.GetChild(i).gameObject);

        float currentRowHeight = GetRowHeight();
        float currentStepWidth = GetStepWidth();

        // Size background to the current viewport so the grid redraws with layout changes.
        gridBackground.anchorMin = new Vector2(0, 1);
        gridBackground.anchorMax = new Vector2(0, 1);
        gridBackground.pivot = new Vector2(0, 1);
        gridBackground.anchoredPosition = Vector2.zero;
        gridBackground.sizeDelta = new Vector2(steps * currentStepWidth, rows * currentRowHeight);

        if (notesLayer != null)
        {
            notesLayer.anchorMin = new Vector2(0, 1);
            notesLayer.anchorMax = new Vector2(0, 1);
            notesLayer.pivot = new Vector2(0, 1);
            notesLayer.anchoredPosition = Vector2.zero;
            notesLayer.sizeDelta = gridBackground.sizeDelta;
        }

        SyncKeyboardPanelHeight();

        // Horizontal lines
        for (int r = 0; r <= rows; r++)
        {
            float y = -r * currentRowHeight;
            CreateLine($"H_{r}", 0, y, steps * currentStepWidth, 2f, thin);
        }

        // Vertical lines
        int stepsPerBar = stepsPerBeat * beatsPerBar;

        for (int s = 0; s <= steps; s++)
        {
            float x = s * currentStepWidth;

            Color c = thin;
            float w = 2f;

            if (s % stepsPerBar == 0) { c = bar;  w = 3f; }
            else if (s % stepsPerBeat == 0) { c = beat; w = 2.5f; }

            CreateLine($"V_{s}", x, 0, w, rows * currentRowHeight, c);
        }
    }

    void CreateLine(string name, float x, float y, float w, float h, Color c)
    {
        var img = Instantiate(linePrefab, gridBackground);
        img.name = name;
        img.color = c;

        var rt = img.rectTransform;
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);
    }

    private float GetRowHeight()
    {
        if (rows <= 0)
            return rowHeight;

        float availableHeight = pianoRollViewport != null && pianoRollViewport.rect.height > 0f
            ? pianoRollViewport.rect.height
            : keyboardPanel != null && keyboardPanel.rect.height > 0f
                ? keyboardPanel.rect.height
            : rows * rowHeight;

        return availableHeight / rows;
    }

    private float GetStepWidth()
    {
        return stepWidth;
    }

    private void SyncKeyboardPanelHeight()
    {
        if (keyboardPanel == null || pianoRollViewport == null || pianoRollViewport.rect.height <= 0f)
            return;

        float viewportHeight = pianoRollViewport.rect.height;
        var layoutElement = keyboardPanel.GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            layoutElement.minHeight = viewportHeight;
            layoutElement.preferredHeight = viewportHeight;
        }
    }
}
