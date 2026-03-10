using UnityEngine;
using UnityEngine.UI;

public class HeaderSync : MonoBehaviour
{
    [Header("Body Scroll")]
    [SerializeField] private ScrollRect bodyScroll; // RollScrollView

    [Header("Header")]
    [SerializeField] private RectTransform headerContent; // GridHeaderNumbers (the moving content)
    [SerializeField] private RectTransform headerViewport; // GridHeaderViewport (masked area)

    [Header("Body Content")]
    [SerializeField] private RectTransform bodyContent; // RollContent
    [SerializeField] private RectTransform bodyViewport; // Viewport

    private float _lastBodyWidth = -1f;

    void LateUpdate()
    {
        if (!bodyScroll || !headerContent || !headerViewport || !bodyContent || !bodyViewport) return;

        // Keep header width in sync with the scrollable body so bar numbers exist
        float bodyWidth = bodyContent.rect.width;
        if (bodyWidth > 0f && !Mathf.Approximately(_lastBodyWidth, bodyWidth))
        {
            var size = headerContent.sizeDelta;
            size.x = bodyWidth;
            headerContent.sizeDelta = size;
            _lastBodyWidth = bodyWidth;
        }

        // How far the body can scroll horizontally
        float bodyScrollableWidth = bodyContent.rect.width - bodyViewport.rect.width;
        if (bodyScrollableWidth <= 0f) return;

        // Convert normalized scroll -> pixels
        float bodyX = bodyScroll.horizontalNormalizedPosition * bodyScrollableWidth;

        // Move header content left by same amount
        // (headerContent anchored at left inside headerViewport)
        var pos = headerContent.anchoredPosition;
        pos.x = -bodyX;
        headerContent.anchoredPosition = pos;
    }
}
