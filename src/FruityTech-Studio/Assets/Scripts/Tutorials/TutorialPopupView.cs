using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button closeButton;

    [Header("Positioning")]
    [SerializeField] private RectTransform popupRoot;
    [SerializeField] private RectTransform arrow;

    [Header("Arrows")]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    private Action _onNext;
    private Action _onSkip;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => _onNext?.Invoke());
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(() => _onSkip?.Invoke());
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => _onSkip?.Invoke());
        }

        gameObject.SetActive(false);
    }

    public void Show(
        string title,
        string body,
        bool showNextButton,
        Action onNext,
        Action onSkip)
    {
        _onNext = onNext;
        _onSkip = onSkip;

        if (titleText != null) titleText.text = title;
        if (bodyText != null) bodyText.text = body;
        if (nextButton != null) nextButton.gameObject.SetActive(showNextButton);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetAnchoredPosition(Vector2 anchoredPosition) 
    {
        Debug.Log("SetAnchoredPosition called with " + anchoredPosition);

        if (popupRoot == null)
        {
            Debug.LogError("popupRoot is NULL");
            return;
        }

        popupRoot.anchorMin = new Vector2(0.5f, 0.5f);
        popupRoot.anchorMax = new Vector2(0.5f, 0.5f);
        popupRoot.pivot = new Vector2(0.5f, 0.5f);
        popupRoot.anchoredPosition = anchoredPosition;

        Debug.Log("popupRoot name = " + popupRoot.name + ", new anchoredPosition = " + popupRoot.anchoredPosition);
    }
    public void SetArrow(TutorialArrowSide side, Vector2 offset)
    {
        if (arrow == null)
            return;

        if (side == TutorialArrowSide.None)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);

        arrow.anchorMin = new Vector2(0.5f, 0.5f);
        arrow.anchorMax = new Vector2(0.5f, 0.5f);
        arrow.pivot = new Vector2(0.5f, 0.5f);

        float halfW = popupRoot.rect.width * 0.5f;
        float halfH = popupRoot.rect.height * 0.5f;

        switch (side)
        {
            case TutorialArrowSide.Left:
                arrow.anchoredPosition = new Vector2(-halfW - 12f, 0f) + offset;
                arrow.localRotation = Quaternion.Euler(0f, 0f, 180f);
                break;

            case TutorialArrowSide.Right:
                arrow.anchoredPosition = new Vector2(halfW + 12f, 0f) + offset;
                arrow.localRotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            case TutorialArrowSide.Top:
                arrow.anchoredPosition = new Vector2(0f, halfH + 12f) + offset;
                arrow.localRotation = Quaternion.Euler(0f, 0f, 90f);
                break;

            case TutorialArrowSide.Bottom:
                arrow.anchoredPosition = new Vector2(0f, -halfH - 12f) + offset;
                arrow.localRotation = Quaternion.Euler(0f, 0f, -90f);
                break;
        }
    }
    public void SetArrowVisibility(bool showLeft, bool showRight)
    {
        if (leftArrow != null) leftArrow.SetActive(showLeft);
        if (rightArrow != null) rightArrow.SetActive(showRight);
    }

}