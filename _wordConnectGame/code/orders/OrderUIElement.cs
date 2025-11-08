using Coffee.UIExtensions;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderUIElement : MonoBehaviour
{
    public bool isActive = true;
    [Space]
    public PlantObject.plantType plantType;
    public TextMeshProUGUI amountText;
    public RectTransform content;
    [Space]
    public Image progressImage;
    public Image iconImage;
    public UIParticleAttractor particleAttractor;
    int currentHarvestAmount;
    int requiedHarvestAmount;

    private void Start()
    {
        if (particleAttractor != null)
        {
            particleAttractor.onAttracted.AddListener(OnAttracted);
        }
    }

    public void SetOrderDetails()
    {
        HarvestOrder order = new HarvestOrder
        {
            currentHarvestAmount = 0,
            requiredHarvestAmount = UnityEngine.Random.Range(5, 10),
            //plantType = (PlantObject.plantType)i
        };

        currentHarvestAmount = order.currentHarvestAmount;
        requiedHarvestAmount = order.requiredHarvestAmount;
        amountText.text = $"{currentHarvestAmount}/{requiedHarvestAmount}";

        UpdateUIElement();
    }



    public void UpdateUIElement(bool increment = false)
    {
        if (increment)
        {
            currentHarvestAmount++;
        }

        if (currentHarvestAmount >= requiedHarvestAmount)
        {
            currentHarvestAmount = requiedHarvestAmount;
            isActive = false;
            OnCompleteOrder();
            return;
        }

        if (amountText != null)
        {
            amountText.text = $"{currentHarvestAmount}/{requiedHarvestAmount}";
        }

        if (progressImage != null)
        {
            progressImage.fillAmount = (float)currentHarvestAmount / requiedHarvestAmount;
        }
    }


    void OnCompleteOrder()
    {
        PlayHideFx();
        isActive = false;
        Invoke(nameof(PlayShowFx), 1f); // Delay to allow hide animation to complete
    }



    Tween punchTween;
    void OnAttracted()
    {
        UpdateUIElement(true);

        if (punchTween == null)
        {
            punchTween = iconImage.transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 5, .5f).OnComplete(() => punchTween = null);

        }

        if (punchTween.IsPlaying())
        {
            return;
        }

        punchTween.Play();
    }

    [Button("Set name")]
    public void SetName()
    {
        gameObject.name = $"{plantType} Order UI Element";
    }


    [Button]
    public void PlayHideFx()
    {
        isActive = false;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(content.DOAnchorPosY(90, .4f).SetEase(Ease.InBack))
            .Insert(0, content.DOScale(Vector3.zero, sequence.Duration()).SetDelay(.25f));

        content.GetComponent<CanvasGroup>().DOFade(0, sequence.Duration()).SetDelay(.2f);
        sequence.OnComplete(() =>
        {
            content.gameObject.SetActive(false);
        });
    }


    [Button]
    public void PlayShowFx()
    {
        transform.SetAsLastSibling();
        content.gameObject.SetActive(true);
        content.anchoredPosition = new Vector2(0, 0);
        content.DOScale(Vector3.one, .25f).SetEase(Ease.OutBack)
        .OnComplete
        (
            () =>
            {
                SceneRegistry.Instance.harvestOrdersHandler.SetActiveOrder();
                //print($"Order UI Element is active: " + SceneRegistry.Instance.harvestOrdersHandler.GetActiveOrder().isActive);
            }
        );
        content.GetComponent<CanvasGroup>().DOFade(1, .25f);
        SetOrderDetails();
    }
}
