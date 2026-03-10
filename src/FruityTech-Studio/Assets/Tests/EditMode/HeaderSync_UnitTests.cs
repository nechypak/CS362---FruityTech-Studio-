using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class HeaderSync_UnitTests
{
    [Test]
    public void LateUpdate_MovesHeaderContentWithScroll()
    {
        var scrollGO = new GameObject();
        var scroll = scrollGO.AddComponent<ScrollRect>();

        var bodyContent = new GameObject().AddComponent<RectTransform>();
        var bodyViewport = new GameObject().AddComponent<RectTransform>();

        var headerContent = new GameObject().AddComponent<RectTransform>();
        var headerViewport = new GameObject().AddComponent<RectTransform>();

        bodyContent.sizeDelta = new Vector2(1000, 100);
        bodyViewport.sizeDelta = new Vector2(400, 100);

        // ScrollRect requires content/viewport before setting normalized positions.
        scroll.content = bodyContent;
        scroll.viewport = bodyViewport;
        scroll.horizontalNormalizedPosition = 0.5f;

        var syncGO = new GameObject();
        var sync = syncGO.AddComponent<HeaderSync>();

        typeof(HeaderSync).GetField("bodyScroll", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sync, scroll);
        typeof(HeaderSync).GetField("bodyContent", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sync, bodyContent);
        typeof(HeaderSync).GetField("bodyViewport", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sync, bodyViewport);
        typeof(HeaderSync).GetField("headerContent", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sync, headerContent);
        typeof(HeaderSync).GetField("headerViewport", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sync, headerViewport);

        sync.Invoke("LateUpdate", 0);

        Assert.Less(headerContent.anchoredPosition.x, 0f);
    }
}
