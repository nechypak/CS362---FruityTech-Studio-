using TMPro;
using UnityEngine;

public class WindowControlsUI : MonoBehaviour
{
    private const int AspectWidth = 16;
    private const int AspectHeight = 9;
    private const float AspectRatio = AspectWidth / (float)AspectHeight;

    [Header("Optional UI")]
    [SerializeField] private TMP_Text windowModeButtonText;

    [Header("Startup")]
    [SerializeField] private bool startFullscreen = false;

    [Header("Windowed Size")]
    [SerializeField] private int windowedWidth = 1280;
    [SerializeField] private int windowedHeight = 720;

    private int fullscreenWidth;
    private int fullscreenHeight;

    private void Start()
    {
        NormalizeWindowedSizeToAspect();

        var fullscreenResolution = GetBestFullscreenResolution16By9();
        fullscreenWidth = fullscreenResolution.x;
        fullscreenHeight = fullscreenResolution.y;

        if (startFullscreen)
            SetFullscreen();
        else
            SetWindowed();

        RefreshWindowButtonVisual();
    }

    public void OnClickCloseApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickToggleWindowMode()
    {
        if (Screen.fullScreen)
            SetWindowed();
        else
            SetFullscreen();

        RefreshWindowButtonVisual();
    }

    private void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
    }

    private void SetFullscreen()
    {
#if UNITY_STANDALONE_WIN
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.SetResolution(fullscreenWidth, fullscreenHeight, FullScreenMode.ExclusiveFullScreen);
#else
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(fullscreenWidth, fullscreenHeight, FullScreenMode.FullScreenWindow);
#endif
    }

    private void RefreshWindowButtonVisual()
    {
        if (windowModeButtonText == null) return;

        windowModeButtonText.text = Screen.fullScreen ? "—" : "▢";
    }

    private void NormalizeWindowedSizeToAspect()
    {
        if (windowedWidth <= 0 || windowedHeight <= 0)
        {
            windowedWidth = 1280;
            windowedHeight = 720;
            return;
        }

        if (IsSixteenByNine(windowedWidth, windowedHeight)) return;

        windowedHeight = Mathf.RoundToInt(windowedWidth / AspectRatio);
        if (windowedHeight <= 0)
            windowedHeight = 720;
    }

    private static bool IsSixteenByNine(int width, int height)
    {
        if (width <= 0 || height <= 0) return false;
        return Mathf.Abs((width / (float)height) - AspectRatio) < 0.01f;
    }

    private static Vector2Int GetBestFullscreenResolution16By9()
    {
        var targetWidth = Display.main.systemWidth;
        var targetHeight = Display.main.systemHeight;

        var bestWidth = 0;
        var bestHeight = 0;
        var bestArea = 0;

        foreach (var resolution in Screen.resolutions)
        {
            if (!IsSixteenByNine(resolution.width, resolution.height)) continue;
            if (resolution.width > targetWidth || resolution.height > targetHeight) continue;

            var area = resolution.width * resolution.height;
            if (area <= bestArea) continue;

            bestArea = area;
            bestWidth = resolution.width;
            bestHeight = resolution.height;
        }

        if (bestArea > 0)
            return new Vector2Int(bestWidth, bestHeight);

        var width = targetWidth;
        var height = Mathf.RoundToInt(width / AspectRatio);
        if (height > targetHeight)
        {
            height = targetHeight;
            width = Mathf.RoundToInt(height * AspectRatio);
        }

        return new Vector2Int(width, height);
    }
}
