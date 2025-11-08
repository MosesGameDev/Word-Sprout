using Coffee.UIEffects;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HarvestOrderUIE : MonoBehaviour
{
    public bool isActive = false;

    [System.Serializable]
    public class HarvestRequirements
    {

        public Image plantImage;
        public TextMeshProUGUI countText;
        public RectTransform content;
        public PlantObject.plantType plantType;
        public HarvestOrder order;
    }

    [Title("Characters")]
    public Image characterImage;
    public int characterIndex = 0;
    public Sprite[] characterSprites;

    [Title("Background")]
    public Image backgroundImage;
    public Sprite activeBackground;
    public Sprite inactiveBackground;
    public RectTransform inactiveSlots;

    [Title("Content")]
    public RectTransform content;


    [Title("OrderBackground")]
    public Image orderBackground;
    public Sprite activeOrderBackground;
    public Sprite inactiveOrderBackground;
    public Sprite completedOrderBackground;

    [Title("Orders")]
    public int orderCount = 1;
    public List<HarvestOrder> orders = new List<HarvestOrder>();
    public HarvestRequirements[] harvestRequirements;
    [Space]
    public Sprite[] plantSprites;

    [Space]
    public ParticleSystem coinFx;


    [Button]
    public void SetInactive()
    {
        isActive = false;
        if (backgroundImage != null)
        {
            backgroundImage.sprite = inactiveBackground;
        }
        if (orderBackground != null)
        {
            orderBackground.sprite = inactiveOrderBackground;
        }

        characterImage.GetComponent<UIEffect>().toneIntensity = 1;

        foreach (var item in harvestRequirements)
        {
            item.content.gameObject.SetActive(false);
        }
        inactiveSlots.gameObject.SetActive(true);
        content.localScale = Vector3.one * 0.9f;
        characterImage.GetComponent<RectTransform>().DOKill();
    }

    [Button]
    public void SetActive()
    {
        isActive = true;
        if (backgroundImage != null)
        {
            backgroundImage.sprite = activeBackground;
        }
        if (orderBackground != null)
        {
            orderBackground.sprite = activeOrderBackground;
        }

        inactiveSlots.gameObject.SetActive(false);
        characterImage.GetComponent<UIEffect>().toneIntensity = 0;
        SetOrder();

        characterImage.GetComponent<RectTransform>().DOAnchorPosY(48, .7f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        content.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);

    }

    [Button]
    public void SetOrder()
    {
        if (!isActive)
        {
            SetActive();
        }

        orders.Clear();

        // Get all plant types as a list we can pull from
        List<PlantObject.plantType> availableTypes = new List<PlantObject.plantType>(
            (PlantObject.plantType[])System.Enum.GetValues(typeof(PlantObject.plantType))
        );

        for (int i = 0; i < orderCount; i++)
        {
            if (availableTypes.Count == 0)
            {
                Debug.LogWarning("Not enough unique plant types for the number of orders requested.");
                break;
            }

            // Pick a random unique type from the available list
            int randomIndex = Random.Range(0, availableTypes.Count);
            PlantObject.plantType chosenType = availableTypes[randomIndex];
            availableTypes.RemoveAt(randomIndex); // remove so it won’t be reused

            HarvestOrder order = new HarvestOrder
            {
                currentHarvestAmount = 0,
                requiredHarvestAmount = Random.Range(4, 8),
                plantType = chosenType
            };

            orders.Add(order);
            harvestRequirements[i].order = order;
        }

        if (orders.Count > 0)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                harvestRequirements[i].content.gameObject.SetActive(true);
                harvestRequirements[i].plantType = orders[i].plantType;
                harvestRequirements[i].plantImage.sprite = plantSprites[(int)orders[i].plantType];
                harvestRequirements[i].countText.text = $"{orders[i].currentHarvestAmount}/{orders[i].requiredHarvestAmount}";
            }
        }

        characterImage.sprite = characterSprites[characterIndex];
    }

    public void SetCharacter(int index)
    {
        characterIndex = index;
        characterImage.sprite = characterSprites[characterIndex];
    }

    [Button]
    public void CompleteOrder()
    {
        
        orderBackground.sprite = completedOrderBackground;

        Sequence sequence = DOTween.Sequence();

        sequence.SetDelay(1);

        sequence.Append(content.DOPunchScale(Vector3.one * .5f, .5f, 5, 1).OnComplete(() => { coinFx.Play(); }))
            .Append(content.DOAnchorPosY(200, 0.5f).SetDelay(1.25f))
            .Join(content.DOScale(Vector3.zero, 0.3f).SetDelay(.2f));

        sequence.OnComplete(() => { SetInactive(); OnCompleteOrder(); });
    }


    void OnCompleteOrder()
    {

        isActive = false;
        transform.SetAsLastSibling();
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
        Invoke(nameof(SetOrder), 1f); // Delay to allow hide animation to complete
    }

    [Button]
    public void Show()
    {
        SetOrder();
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
        content.DOScale(Vector3.one * 0.9f, 0.3f).SetEase(Ease.OutBack);
    }

    public bool IsOrderComplete()
    {
        foreach (var order in orders)
        {
            if (order.currentHarvestAmount < order.requiredHarvestAmount)
            {
                return false; // still missing something
            }
        }
        return true;
    }


    public void UpdateUIElement(PlantObject.plantType plantType)
    {
        if (IsOrderComplete())
        {
            CompleteOrder();
            return;
        }

        for (int i = 0; i < harvestRequirements.Length; i++)
        {
            if (harvestRequirements[i].plantType != plantType) continue;

            // increment only while still incomplete
            if (orders[i].currentHarvestAmount < orders[i].requiredHarvestAmount &&
                harvestRequirements[i].content.gameObject.activeInHierarchy)
            {
                int from = orders[i].currentHarvestAmount;
                int step = Random.Range(2, 4);
                int to = Mathf.Min(from + step, orders[i].requiredHarvestAmount);
                harvestRequirements[i].plantImage.transform.DOKill();
                harvestRequirements[i].plantImage.transform.localScale = Vector3.one;
                harvestRequirements[i].plantImage.transform.DOPunchScale(Vector3.one * 0.25f, 0.4f, 5, 1);
                orders[i].currentHarvestAmount = to;

                DOTween.To(() => from, x =>
                {
                    from = x;
                    harvestRequirements[i].countText.text = $"{from}/{orders[i].requiredHarvestAmount}";
                }, to, 0.5f).SetEase(Ease.OutQuad);

                if (IsOrderComplete())
                {
                    CompleteOrder(); 
                }
            }
            return;
        }
    }
}