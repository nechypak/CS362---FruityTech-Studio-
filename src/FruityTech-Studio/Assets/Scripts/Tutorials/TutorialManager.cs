using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TutorialPopupView popupView;
    [SerializeField] private TutorialHighlighter highlighter;

    [Header("Steps")]
    [SerializeField] private List<TutorialStep> steps = new();

    [Header("Behavior")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool onlyShowOnce = false;

    private int _currentIndex = -1;
    private bool _isRunning;
    private bool _waitingForAction;

    private const string CompletedKey = "tutorial_completed";

    private void Start()
    {
        if (!playOnStart)
            return;

        if (onlyShowOnce && IsCompleted())
            return;

        StartTutorial();
    }

    public void StartTutorial()
    {
        if (steps == null || steps.Count == 0 || popupView == null)
        {
            Debug.LogWarning("TutorialManager: Missing steps or popupView.");
            return;
        }

        _isRunning = true;
        _currentIndex = -1;
        ShowNextStep();
    }

    public void SkipTutorial()
    {
        _isRunning = false;
        _waitingForAction = false;

        popupView.Hide();
        highlighter?.Clear();
    }

    public void ShowNextStep()
    {
        if (!_isRunning)
            return;

        _currentIndex++;

        if (_currentIndex >= steps.Count)
        {
            CompleteTutorial();
            return;
        }

        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        var step = steps[_currentIndex];

        _waitingForAction = step.waitForAction;

        popupView.Show(
            step.title,
            step.body,
            true,
            OnNextClicked,
            SkipTutorial
        );

        popupView.SetArrowVisibility(step.showLeftArrow, step.showRightArrow);

        Canvas.ForceUpdateCanvases();
        popupView.SetAnchoredPosition(step.popupAnchoredPosition);

        if (step.focusTarget != null)
            highlighter?.Highlight(step.focusTarget);
        else
            highlighter?.Clear();
    }

    private void OnNextClicked()
    {
        if (!_isRunning)
            return;

        _waitingForAction = false;
        ShowNextStep();
    }

    private void CompleteTutorial()
    {
        _isRunning = false;
        _waitingForAction = false;

        popupView.Hide();
        highlighter?.Clear();

        if (onlyShowOnce)
            SetCompleted(true);

        Debug.Log("Tutorial complete.");
    }

    public void NotifyPlayPressed()
    {
        Debug.Log("TutorialManager.NotifyPlayPressed()");
        TryCompleteAction(TutorialStepType.PressPlay);
    }

    public void NotifyNotePlaced()
    {
        Debug.Log("TutorialManager.NotifyNotePlaced()");
        TryCompleteAction(TutorialStepType.PlaceNote);
    }

    public void NotifyUndoUsed()
    {
        Debug.Log("TutorialManager.NotifyUndoUsed()");
        TryCompleteAction(TutorialStepType.UseUndo);
    }

    private void TryCompleteAction(TutorialStepType actionType)
    {
        if (!_isRunning || !_waitingForAction)
            return;

        if (_currentIndex < 0 || _currentIndex >= steps.Count)
            return;

        var step = steps[_currentIndex];

        Debug.Log($"Tutorial action received: {actionType}, current step expects: {step.stepType}");

        if (step.stepType != actionType)
            return;

        _waitingForAction = false;
        ShowNextStep();
    }

    public static bool IsCompleted()
    {
        return PlayerPrefs.GetInt(CompletedKey, 0) == 1;
    }

    public static void SetCompleted(bool completed)
    {
        PlayerPrefs.SetInt(CompletedKey, completed ? 1 : 0);
        PlayerPrefs.Save();
    }

    [ContextMenu("Reset Tutorial Completion")]
    public void ResetCompletion()
    {
        SetCompleted(false);
    }
}