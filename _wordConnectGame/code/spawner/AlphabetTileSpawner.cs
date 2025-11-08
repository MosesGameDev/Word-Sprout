using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AlphabetTileSpawner : MonoBehaviour
{
    [SerializeField] private AlphabetTile alphabetTilePrefab;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform collectionPosition;

    [Space]
    [SerializeField] public List<AlphabetTile> spawnedAlpabetTiles = new List<AlphabetTile>();

    public IObjectPool<AlphabetTile> alphabetTilePool;
    bool collectionsCheck = true;
    int defaultPoolSize = 50;
    int maxPoolSize = 52;

    private void Awake()
    {
        alphabetTilePool = new ObjectPool<AlphabetTile>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,
            collectionsCheck, defaultPoolSize, maxPoolSize);
    }


    public Transform GetSpawnPosition()
    {
        return spawnPosition;
    }


    public void PlayAlphabetTilesCollectionFx()
    {
        for (int i = 0; i < spawnedAlpabetTiles.Count; i++)
        {
            spawnedAlpabetTiles[i].MoveToCollectionPoint(collectionPosition.position, i * 0.1f );
        }
    }


    AlphabetTile CreatePooledItem()
    {
        AlphabetTile tile = Instantiate(alphabetTilePrefab);
        return tile;
    }

    void OnReturnedToPool(AlphabetTile tile)
    {
        tile.Reset();
        tile.gameObject.SetActive(false);
    }

    void OnTakeFromPool(AlphabetTile tile)
    {
        tile.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(AlphabetTile tile)
    {
        Destroy(tile.gameObject);
    }


}
