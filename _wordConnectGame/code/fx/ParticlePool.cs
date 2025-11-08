using Coffee.UIExtensions;
using DamageNumbersPro;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    // Singleton instance
    private static ParticlePool _instance;
    public static ParticlePool Instance => _instance;

    // Dictionary to store different pools for different particle effect prefabs
    private Dictionary<string, Queue<ParticleSystem>> poolDictionary = new Dictionary<string, Queue<ParticleSystem>>();

    // Dictionary to track which prefab a particle system belongs to
    private Dictionary<ParticleSystem, string> prefabLookup = new Dictionary<ParticleSystem, string>();

    [System.Serializable]
    public class PoolItem
    {
        public string key;
        public ParticleSystem prefab;
        public int initialSize = 10;
    }

    public DamageNumber plantLevelUpFX;

    [Space]
    public UIParticle[] cornUIParticles;
    public UIParticle[] carrotUIParticles;
    public UIParticle[] sunflowerUIParticles;
    public UIParticle[] mushroomUIParticles;
    public UIParticle[] cauliflowerUIParticles;
    public UIParticle[] broccoliUIParticles;

    [Space]
    // Define your particle effect prefabs and initial pool sizes in the inspector
    public List<PoolItem> poolItems = new List<PoolItem>();

    private void Awake()
    {
        // Set up singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize all pools
        foreach (PoolItem item in poolItems)
        {
            // Create an empty queue
            Queue<ParticleSystem> objectPool = new Queue<ParticleSystem>();

            // Fill the queue with inactive objects
            for (int i = 0; i < item.initialSize; i++)
            {
                ParticleSystem obj = CreateNewParticleSystem(item.prefab, item.key);
                objectPool.Enqueue(obj);
            }

            // Add the queue to the pool dictionary
            poolDictionary.Add(item.key, objectPool);
        }
    }

    public void PlayPlantLevelUpFX(Vector3 _position, int level)
    {
        if (plantLevelUpFX != null)
        {
            plantLevelUpFX.Spawn(_position, newLeftText: $"+{1}");
        }
    }



    private ParticleSystem CreateNewParticleSystem(ParticleSystem prefab, string key)
    {
        ParticleSystem newSystem = Instantiate(prefab, transform);
        newSystem.gameObject.SetActive(false);
        prefabLookup.Add(newSystem, key);
        return newSystem;
    }

    public ParticleSystem SpawnFromPool(string key, Vector3 position, Quaternion rotation)
    {
        // Check if the key exists in the pool
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key {key} doesn't exist.");
            return null;
        }

        // Get a reference to the queue
        Queue<ParticleSystem> objectPool = poolDictionary[key];

        // If there's no object available in the pool, create a new one
        if (objectPool.Count == 0)
        {
            // Find the prefab to instantiate
            ParticleSystem prefabToSpawn = poolItems.Find(item => item.key == key).prefab;
            ParticleSystem newObj = CreateNewParticleSystem(prefabToSpawn, key);

            // Set position and activate
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.gameObject.SetActive(true);

            // Play the particle effect
            newObj.Play();

            return newObj;
        }

        // Otherwise, dequeue an object from the pool
        ParticleSystem objToSpawn = objectPool.Dequeue();

        // If the object was destroyed for some reason, create a new one
        if (objToSpawn == null)
        {
            ParticleSystem prefabToSpawn = poolItems.Find(item => item.key == key).prefab;
            objToSpawn = CreateNewParticleSystem(prefabToSpawn, key);
        }

        // Set position and activate
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.gameObject.SetActive(true);

        // Play the particle effect
        objToSpawn.Play();

        return objToSpawn;
    }

    public void ReturnToPool(ParticleSystem particleSystem)
    {
        // Make sure we know which pool this belongs to
        if (!prefabLookup.ContainsKey(particleSystem))
        {
            Debug.LogWarning("Tried to return an object that isn't pooled!");
            return;
        }

        string key = prefabLookup[particleSystem];
        particleSystem.gameObject.SetActive(false);
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Return to the appropriate queue
        poolDictionary[key].Enqueue(particleSystem);
    }

    private Dictionary<UIParticle[], int> lastUsedIndexLookup = new Dictionary<UIParticle[], int>();

    public UIParticle GetUIParticle(PlantObject.plantType plantType)
    {
        UIParticle[] particles = null;
        switch (plantType)
        {
            case PlantObject.plantType.Corn: particles = cornUIParticles; break;
            case PlantObject.plantType.Carrot: particles = carrotUIParticles; break;
            case PlantObject.plantType.Sunflower: particles = sunflowerUIParticles; break;
            case PlantObject.plantType.Mushroom: particles = mushroomUIParticles; break;
            case PlantObject.plantType.Cauliflower: particles = cauliflowerUIParticles; break;
            default: return null;
        }

        if (particles == null || particles.Length == 0) return null;

        // Start cycling from last index used (round-robin approach)
        int startIndex = lastUsedIndexLookup.ContainsKey(particles) ? lastUsedIndexLookup[particles] : 0;

        for (int i = 0; i < particles.Length; i++)
        {
            int idx = (startIndex + i) % particles.Length;
            if (!particles[idx].particles[0].isPlaying)
            {
                lastUsedIndexLookup[particles] = (idx + 1) % particles.Length;
                return particles[idx];
            }
        }

        return null;
    }
    public void PositionUIParticle(UIParticle uIParticle, Transform targetTransform)
    {
        // Make sure you're using a Screen Space - Overlay or Screen Space - Camera canvas
        Canvas canvas = uIParticle.GetComponentInParent<Canvas>();
        RectTransform rectTransform = uIParticle.GetComponent<RectTransform>();

        // Convert world position of child to canvas space
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, targetTransform.position);

        Vector2 localPoint;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out localPoint);

        rectTransform.anchoredPosition = localPoint;

        uIParticle.gameObject.SetActive(true);
        uIParticle.Play();
    }
}
