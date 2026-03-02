using TMPro;
using UnityEngine;

public class LeftKeysBuilder : MonoBehaviour
{
    public RectTransform parent;
    public GameObject labelPrefab;
    public string[] noteIdsLowToHigh;
    public float rowHeight = 40f;

    private void Start()
    {
        Build();
    }

    public void Build()
    {
        // очистка
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);

        // сверху High, снизу Low
        for (int i = noteIdsLowToHigh.Length - 1; i >= 0; i--)
        {
            var go = Instantiate(labelPrefab, parent);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rowHeight);

            var txt = go.GetComponent<TMP_Text>();
            txt.text = noteIdsLowToHigh[i];
        }
    }
}