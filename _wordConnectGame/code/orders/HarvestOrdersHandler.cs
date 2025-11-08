using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static PlantObject;

public class HarvestOrdersHandler : MonoBehaviour
{
    public int activeOrders = 4;
    public List<HarvestOrderUIE> orderUIElements = new List<HarvestOrderUIE>();
    bool isInitialized = false;

    private void OnEnable()
    {
        LevelManager.OnLevelManagerInitialized += LevelManager_OnLevelManagerInitialized;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelManagerInitialized -= LevelManager_OnLevelManagerInitialized;
    }

    private void LevelManager_OnLevelManagerInitialized(WordLevelScriptableObject obj)
    {
        if (isInitialized)
        {
            return;
        }
        Initialize();
    }

    private void Start()
    {
        for (int i = 0; i < orderUIElements.Count; i++)
        {
            orderUIElements[i].gameObject.SetActive(false);
        }
    }

    [Button("Initialize Orders")]
    public void Initialize()
    {
        int spriteCount = (orderUIElements.Count > 0 && orderUIElements[0].characterSprites != null)
            ? orderUIElements[0].characterSprites.Length
            : 0;


        List<int> indices = new List<int>(spriteCount);
        for (int x = 0; x < spriteCount; x++) indices.Add(x);

        for (int i = 0; i < indices.Count; i++)
        {
            int rand = Random.Range(i, indices.Count);
            (indices[i], indices[rand]) = (indices[rand], indices[i]);
        }

        int assignCount = Mathf.Min(activeOrders, orderUIElements.Count, indices.Count);


        for (int i = assignCount - 1; i >= 0; i--)
        {
            orderUIElements[i].gameObject.SetActive(true);
            int index = indices[i];
            orderUIElements[i].SetCharacter(index);
            orderUIElements[i].SetActive();
        }

        isInitialized = true;
    }

    public HarvestOrderUIE GetActiveOrder(PlantObject.plantType type)
    {
        if (orderUIElements == null || orderUIElements.Count == 0)
        {
            Debug.LogError("[HarvestOrdersHandler] orderUIElements is null or empty.");
            return null;
        }

        for (int i = 0; i < orderUIElements.Count; i++)
        {
            var ui = orderUIElements[i];
            if (ui == null)
            {
                Debug.LogWarning($"[HarvestOrdersHandler] orderUIElements[{i}] is null.");
                continue;
            }
            if (!ui.isActive) continue;

            var reqs = ui.harvestRequirements;
            if (reqs == null || reqs.Length == 0)
            {
                Debug.LogWarning($"[HarvestOrdersHandler] '{ui.name}' has no harvestRequirements.");
                continue;
            }

            for (int j = 0; j < reqs.Length; j++)
            {
                var req = reqs[j];
                if (req == null)
                {
                    Debug.LogWarning($"[HarvestOrdersHandler] '{ui.name}'.harvestRequirements[{j}] is null.");
                    continue;
                }
                if (req.plantType != type) continue;

                var o = req.order;
                if (o == null)
                {
                    Debug.LogWarning($"[HarvestOrdersHandler] '{ui.name}'.harvestRequirements[{j}].order is null.");
                    continue;
                }

                // Support either field name if you haven’t finished the rename everywhere yet.
                int required = 0;
#if UNITY_EDITOR
                try { required = o.requiredHarvestAmount; }
                catch { required = (int)o.GetType().GetField("requiedHarvestAmount").GetValue(o); }
#else
            required = o.requiredHarvestAmount;
#endif

                if (o.currentHarvestAmount < required)
                    return ui;
            }
        }

        return null;
    }

    public void SetActiveOrder()
    {
        transform.GetChild(0).GetComponent<OrderUIElement>().isActive = true;
    }

    public bool isOrderActive(PlantObject.plantType plantType)
    {
        foreach (HarvestOrderUIE orderUIElement in orderUIElements)
        {
            if (orderUIElement.isActive)
            {
                return true;
            }
        }
        return false;
    }

}
