using Coffee.UIExtensions;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlantObject : MonoBehaviour
{
    public enum plantType
    {
        Corn,
        Carrot,
        Sunflower,
        Mushroom,
        Cauliflower
        
    }
    public plantType type;
    public int currentLevel;
    public WorldGridTile worldGridTile;
    [Space]
    public GameObject[] plantLevels;

    [Space]
    public float punchScale = 0.0125f;
    public float scaleFactor = 0.01f;



    [Button("Set Plant Level")]
    public void Grow()
    {
        worldGridTile = transform.parent.GetComponent<WorldGridTile>();

        if (currentLevel < 0 || currentLevel >= plantLevels.Length)
        {
            Debug.LogError("Invalid plant level: " + currentLevel);
            return;
        }

        foreach (var plant in plantLevels)
        {
            if (plant == null)
            {
                continue;
            }
            if (plant.activeInHierarchy)
            {
                plant.transform.DOKill();
                plant.transform.DOScale(Vector3.zero, 0.5f);
            }
            plant.SetActive(false);
        }

        plantLevels[currentLevel].SetActive(true);

        plantLevels[currentLevel].transform.DOScale(Vector3.one * punchScale, 0.8f).SetEase(Ease.OutBounce)
            .OnComplete(() => plantLevels[currentLevel].transform.DOScale(Vector3.one * scaleFactor, 0.2f));

        if (currentLevel < plantLevels.Length - 1)
        {
            currentLevel++;
            ParticlePool.Instance.PlayPlantLevelUpFX(plantLevels[currentLevel].transform.position, currentLevel + 1);
        }
        else
        {
            if(SceneRegistry.Instance.harvestOrdersHandler.GetActiveOrder(type) != null)
            {
                SceneRegistry.Instance.harvestOrdersHandler.GetActiveOrder(type).UpdateUIElement(type);
            }
            worldGridTile.SetPlant();
            currentLevel = 0;

            UIParticle uip = ParticlePool.Instance.GetUIParticle(type);
            ParticlePool.Instance.PositionUIParticle(uip, plantLevels[currentLevel].transform);
        }
    }

    public void Reset()
    {
        foreach (var plant in plantLevels)
        {
            if (plant.activeInHierarchy)
            {
                plant.transform.DOKill();
                plant.transform.DOScale(Vector3.zero, 0.5f);
            }
            plant.SetActive(false);
        }
        currentLevel = 0;
    }
}
