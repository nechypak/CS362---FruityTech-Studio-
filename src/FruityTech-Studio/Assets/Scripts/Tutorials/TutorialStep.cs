using System;
using UnityEngine;

public enum TutorialStepType
{
    MessageOnly,
    PressPlay,
    PlaceNote,
    UseUndo
}

public enum TutorialArrowSide
{
    None,
    Left,
    Right,
    Top,
    Bottom
}

[Serializable]
public class TutorialStep
{
    public string title;

    [TextArea(3, 6)]
    public string body;

    public TutorialStepType stepType = TutorialStepType.MessageOnly;

    [Tooltip("Optional UI target to visually highlight during this step.")]
    public RectTransform focusTarget;

    [Header("Popup Position")]
    [Tooltip("Exact anchored position of the popup on the canvas.")]
    public Vector2 popupAnchoredPosition;

    [Header("Behavior")]
    [Tooltip("If true, hides the Next button and waits for the matching tutorial action.")]
    public bool waitForAction = false;

    [Header("Arrow")]
    public TutorialArrowSide arrowSide = TutorialArrowSide.None;
    public Vector2 arrowOffset = Vector2.zero;
}