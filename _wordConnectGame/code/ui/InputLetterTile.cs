using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.UI.Extensions;

public class InputLetterTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    LevelInputManager _levelInputManager;

    [Space]
    public string letter;
    [SerializeField] private bool isSelected;

    [Space]
    [SerializeField] private TextMeshProUGUI letterText;
    [SerializeField] private UISquircle tileBackground;

    public static event System.Action<string, RectTransform> OnInputLetterTileSelected;
    public static event System.Action<string, RectTransform> OnInputLetterTileDeselected;

    // Keep track of active coroutines so we can stop them if needed
    private Coroutine destroyTileCoroutine;

    private void Start()
    {
        _levelInputManager = SceneRegistry.Instance.levelInputManager;
    }

    public void SetLetterText(string _letter)
    {
        letter = _letter;
        letterText.SetText(letter);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!_levelInputManager.inputInitialized)
        {
            return;
        }

        if (isSelected)
        {
            RemoveLetterFromPlayerInput();
            return;
        }

        SetSelected();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (_levelInputManager.inputInitialized)
        {
            return;
        }

        _levelInputManager.inputInitialized = true;
        SetSelected();
    }

    /// <summary>
    /// Deselects tile and removes tile letter from players input
    /// </summary>
    void RemoveLetterFromPlayerInput()
    {
        //Test
        string input = _levelInputManager.playerInput;
        char i = _levelInputManager.playerInput.Last();

        if (i == letter.Last())
        {
            SetDeselected();
        }
    }

    public void SetSelected()
    {
        isSelected = true;

        GetComponent<RectTransform>().DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack);
        OnInputLetterTileSelected?.Invoke(letter, GetComponent<RectTransform>());
    }

    public void SetDeselected()
    {
        StartCoroutine(DelaySetDeselectedCoroutine(0.1f));
    }

    private IEnumerator DelaySetDeselectedCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        isSelected = false;
        GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBounce);

        if (!LevelManager.Instance.levelWordsDictionary.IsInputValid(_levelInputManager.playerInput))
        {
            //print("LN:114");
            OnInputLetterTileDeselected?.Invoke(letter, GetComponent<RectTransform>());
        }
        else
        {
            _levelInputManager.OnCorrectInput();
        }
    }

    public void PlayShakeTween(float sequenceDuration = .3f, bool changeColor = true)
    {
        //Color startColor = tileBackground.color;
        RectTransform rectTransform = GetComponent<RectTransform>();

        Tween t = null;

        if (t == null)
        {
            t = rectTransform.DOShakeRotation(sequenceDuration, 50, 80, 90).OnComplete(() => rectTransform.rotation = new Quaternion());
        }

        if (t.IsPlaying())
        {
            return;
        }

        t = rectTransform.DOShakeRotation(sequenceDuration, 50, 100, 90).OnComplete(() => rectTransform.rotation = new Quaternion());
    }

    public void DestroyTile()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        tileBackground.DOColor(Color.red, 0.5f);

        Vector3 punch = new Vector3(0, 0, -15);
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(rectTransform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack))
            .Insert(0, rectTransform.DOPunchRotation(punch, sequence.Duration(), 8, 1));

        // Replace Invoke with coroutine
        if (destroyTileCoroutine != null)
        {
            StopCoroutine(destroyTileCoroutine);
        }
        destroyTileCoroutine = StartCoroutine(DestroyTileCoroutine(1f));
    }

    // Coroutine to replace the Invoke("DestroyTile", 1f) call
    private IEnumerator DestroyTileCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnDestroyTile();
    }

    private void OnDestroyTile()
    {
        Destroy(gameObject);
    }

    // Make sure to stop any running coroutines when the object is disabled or destroyed
    private void OnDisable()
    {
        if (destroyTileCoroutine != null)
        {
            StopCoroutine(destroyTileCoroutine);
            destroyTileCoroutine = null;
        }
    }
}