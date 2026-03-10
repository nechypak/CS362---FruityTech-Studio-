using TMPro;
using UnityEngine;

public class WindowControlsUI : MonoBehaviour
{
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
        fullscreenWidth = Display.main.systemWidth;
        fullscreenHeight = Display.main.systemHeight;

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
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(fullscreenWidth, fullscreenHeight, FullScreenMode.FullScreenWindow);
    }

    private void RefreshWindowButtonVisual()
    {
        if (windowModeButtonText == null) return;

        windowModeButtonText.text = Screen.fullScreen ? "—" : "▢";
    }
}