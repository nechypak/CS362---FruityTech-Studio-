using UnityEngine;
using UnityEngine.EventSystems;

public class PianoRollController : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Sequencer sequencer;
    public RectTransform gridContent;      // GridContent
    public GameObject noteBlockPrefab;     // Prefabs/NoteBlock

    [Header("Grid visuals")]
    public RectTransform gridLinesParent;  // GridLines
    public GameObject linePrefab;          // Prefabs/Line
    public int beatsPerBar = 4;            // 4/4

    [Header("Grid")]
    public float cellWidth = 50f;          // 1 step
    public float cellHeight = 24f;         // 1 note row
    public int totalSteps = 64;
    public string[] noteIdsLowToHigh;
    public int defaultLengthSteps = 4;

    private void Start()
    {
        ResizeGrid();
        DrawGrid();
    }

    public void ResizeGrid()
    {
        if (gridContent == null || noteIdsLowToHigh == null) return;

        float w = totalSteps * cellWidth;
        float h = noteIdsLowToHigh.Length * cellHeight;
        gridContent.sizeDelta = new Vector2(w, h);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sequencer == null || gridContent == null || noteBlockPrefab == null) return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridContent, eventData.position, eventData.pressEventCamera, out var local))
            return;

        Rect r = gridContent.rect;

        float xFromLeft = local.x - r.xMin;
        float yFromTop = r.yMax - local.y;

        int stepIndex = Mathf.FloorToInt(xFromLeft / cellWidth);
        int rowIndex = Mathf.FloorToInt(yFromTop / cellHeight);

        if (stepIndex < 0 || stepIndex >= totalSteps) return;
        if (rowIndex < 0 || rowIndex >= noteIdsLowToHigh.Length) return;

        // rowIndex=0 значит верхняя строка. Обычно верх — high notes
        string noteId = noteIdsLowToHigh[noteIdsLowToHigh.Length - 1 - rowIndex];

        var e = new NoteEvent
        {
            noteId = noteId,
            stepIndex = stepIndex,
            lengthSteps = defaultLengthSteps,
            velocity = 1f
        };

        sequencer.events.Add(e);
        SpawnBlock(rowIndex, e);
    }

    private void SpawnBlock(int rowIndex, NoteEvent e)
    {
        var go = Instantiate(noteBlockPrefab, gridContent);
        var rt = go.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);

        float x = e.stepIndex * cellWidth;
        float y = rowIndex * cellHeight;

        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(e.lengthSteps * cellWidth, cellHeight);
    }
    
    private void ClearLines()
{
    if (gridLinesParent == null) return;
    for (int i = gridLinesParent.childCount - 1; i >= 0; i--)
        Destroy(gridLinesParent.GetChild(i).gameObject);
}

public void DrawGrid()
{
    if (gridLinesParent == null || linePrefab == null) return;

    ClearLines();

    float width = totalSteps * cellWidth;
    float height = noteIdsLowToHigh.Length * cellHeight;

    // Горизонтальные линии (каждая нота)
    for (int r = 0; r <= noteIdsLowToHigh.Length; r++)
    {
        var go = Instantiate(linePrefab, gridLinesParent);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);

        rt.sizeDelta = new Vector2(width, 1);
        rt.anchoredPosition = new Vector2(0, -r * cellHeight);


    }
    
    for (int s = 0; s <= totalSteps; s++)
        {
            var go = Instantiate(linePrefab, gridLinesParent);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);

            rt.sizeDelta = new Vector2(1, height);
            rt.anchoredPosition = new Vector2(s * cellWidth, 0);

            bool isBeat = (s % 4 == 0);
            bool isBar = (s % (beatsPerBar * 4) == 0); 

            float alpha = isBar ? 0.35f : isBeat ? 0.20f : 0.10f;

            var img = go.GetComponent<UnityEngine.UI.Image>();
            var c = img.color;
            c.a = alpha;
            img.color = c;

            rt.sizeDelta = new Vector2(isBar ? 3 : isBeat ? 2 : 1, height);
        }
}
}