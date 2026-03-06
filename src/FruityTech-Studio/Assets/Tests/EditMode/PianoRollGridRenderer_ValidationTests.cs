using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PianoRollGridRenderer_ValidationTests
{
    // Tests PianoRollGridRenderer.cs, specifically Build()
    [Test]
    public void Build_SetsBackgroundSize_AndCreatesExpectedNumberOfLines()
    {
        var root = new GameObject("GridRoot");
        var bg = root.AddComponent<RectTransform>();

        var linePrefabGO = new GameObject("LinePrefab");
        var linePrefabImg = linePrefabGO.AddComponent<Image>();
        linePrefabGO.AddComponent<RectTransform>();

        var rendererGO = new GameObject("Renderer");
        var renderer = rendererGO.AddComponent<PianoRollGridRenderer>();

        renderer.gridBackground = bg;
        renderer.linePrefab = linePrefabImg;

        renderer.rows = 12;
        renderer.rowHeight = 10f;
        renderer.steps = 16;
        renderer.stepWidth = 5f;

        renderer.stepsPerBeat = 4;
        renderer.beatsPerBar = 4;

        renderer.Build();

        // Validates sizing rule
        Assert.AreEqual(renderer.steps * renderer.stepWidth, bg.sizeDelta.x, 0.0001f);
        Assert.AreEqual(renderer.rows * renderer.rowHeight, bg.sizeDelta.y, 0.0001f);

        // Validates structure: (rows+1) horizontal + (steps+1) vertical lines
        int expectedLines = (renderer.rows + 1) + (renderer.steps + 1);
        Assert.AreEqual(expectedLines, bg.childCount);

        Object.DestroyImmediate(rendererGO);
        Object.DestroyImmediate(linePrefabGO);
        Object.DestroyImmediate(root);
    }
}