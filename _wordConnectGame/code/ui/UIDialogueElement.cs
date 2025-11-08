using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class UIDialogueElement : MonoBehaviour
{
    [Header("Dialogue ID")]
    public string dialogueID;

    [Header("Enable Dialogue On Start")]
    public bool startOpen;

    [Header("Animate Open/Close Tween?")]
    [SerializeField] private bool animatePopUp;
    [SerializeField] private bool animateShrink;

    [Header("Tween properties")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float animationDuration = 0.5f;


    [Space]
    public UnityEvent onShow;
    public UnityEvent onHide;



    [HideInInspector] public bool isOpen;
    CanvasGroup canvasGroup;


    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (startOpen)
        {
            if (canvasGroup.alpha == 0)
            {
                canvasGroup.alpha = 1;
                SetCanvasInteration(true);
            }
        }
        else
        {
            canvasGroup.alpha = 0;
            SetCanvasInteration(false);
        }

    }


    [Button("Show")]
    public void Show()
    {
        if (!isOpen)
        {
            float delay = 0;

            if (animatePopUp)
            {
                delay = fadeDuration;
                PlayPopupAnim(0);
            }

            if (!canvasGroup)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            canvasGroup.DOFade(1, fadeDuration)
            .OnComplete
            (
                () =>
                {
                    SetCanvasInteration(true);
                    onShow.Invoke();
                    isOpen = true;
                }
            );
        }

    }


    [Button("Hide")]

    public void Hide()
    {
        if (animateShrink)
        {
            PlayShrinkAnim(0);
        }


        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.DOFade(0, fadeDuration)
        .OnComplete
        (
            () =>
            {
                SetCanvasInteration(false);
                onHide.Invoke();
                isOpen = false;
            }
        );

    }



    [Button("Add to UI Dialogues Manager")]
    public void AddToUIDialogueManager()
    {
        if (!FindFirstObjectByType<UIDialoguesManager>())
        {
            Debug.LogError("'UIDialoguesManager' NOT FOUND ADD 'UIDialoguesManager' TO THE SCENE");
            return;
        }

        if (!FindFirstObjectByType<UIDialoguesManager>().uIDialogues.Contains(this))
        {
            FindFirstObjectByType<UIDialoguesManager>().uIDialogues.Add(this);
        }
    }

    public void SetCanvasInteration(bool value)
    {
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }


    void PlayPopupAnim(float _delay)
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, animationDuration).SetDelay(_delay).SetEase(Ease.OutBounce);
    }


    void PlayShrinkAnim(float _delay)
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOScale(Vector3.zero, animationDuration).SetDelay(_delay).SetEase(Ease.InBounce, 2);
    }


    public void PlayPunchAnim(float punch = 0.1f)
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one * punch, animationDuration);
    }


}


