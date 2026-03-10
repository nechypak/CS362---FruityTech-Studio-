using UnityEngine;

public class TutorialHighlighter : MonoBehaviour
{
    [SerializeField] private RectTransform highlightFrame;
    [SerializeField] private Canvas rootCanvas;

    private RectTransform _currentTarget;

    public void Highlight(RectTransform target)
    {
        _currentTarget = target;

        if (highlightFrame == null)
            return;

        if (_currentTarget == null)
        {
            highlightFrame.gameObject.SetActive(false);
            return;
        }

        highlightFrame.gameObject.SetActive(true);
        Refresh();
    }

    public void Clear()
    {
        _currentTarget = null;

        if (highlightFrame != null)
            highlightFrame.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_currentTarget != null)
            Refresh();
    }

    private void Refresh()
    {
        if (_currentTarget == null || highlightFrame == null)
            return;

        var targetWorldCorners = new Vector3[4];
        _currentTarget.GetWorldCorners(targetWorldCorners);

        var parent = highlightFrame.parent as RectTransform;
        if (parent == null) return;

        Camera cam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = rootCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            RectTransformUtility.WorldToScreenPoint(cam, targetWorldCorners[0]),
            cam,
            out var minLocal);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            RectTransformUtility.WorldToScreenPoint(cam, targetWorldCorners[2]),
            cam,
            out var maxLocal);

        var size = maxLocal - minLocal;
        highlightFrame.anchoredPosition = minLocal;
        highlightFrame.sizeDelta = size;
    }
}