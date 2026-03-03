using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FixedKeyListFitter : MonoBehaviour
{
    [SerializeField] private int rows = 12;

    private void LateUpdate()
    {
        if (rows <= 0) return;

        var rt = (RectTransform)transform;
        float h = rt.rect.height;
        float rowH = Mathf.Floor(h / rows);

        for (int i = 0; i < rt.childCount; i++)
        {
            var child = rt.GetChild(i) as RectTransform;
            if (!child) continue;

            var le = child.GetComponent<LayoutElement>();
            if (!le) le = child.gameObject.AddComponent<LayoutElement>();

            le.minHeight = rowH;
            le.preferredHeight = rowH;
            le.flexibleHeight = 0;
        }
    }
}