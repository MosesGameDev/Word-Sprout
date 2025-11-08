using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class WorldGrid : MonoBehaviour
{
    [Header("Grid Configuration")]
    [Range(1, 20)] public int rows = 6;
    [Range(1, 20)] public int columns = 7;

    [Space]
    [Range(0.5f, 5f)] public float tileSize = 1f;
    [Range(0f, 2f)] public float tileSpacing = 0f; // Space between tiles

    [Space]
    [Header("Tile Settings")]
    public WorldGridTile tilePrefab;
    [SerializeField] private bool autoCenter = true;
    [SerializeField] private Vector3 gridOffset = Vector3.zero;

    [Space]
    [Header("Runtime Data")]
    [ReadOnly] public List<WorldGridTile> gridTiles = new List<WorldGridTile>();
    public List<WorldGridTile> savedGridTiles = new List<WorldGridTile>();

    // Preview in editor
#if UNITY_EDITOR
    [OnValueChanged("DrawPreviewGrid")]
    [PropertySpace(10)]
    [Button("Update Preview", ButtonSizes.Large)]
    public void DrawPreviewGrid()
    {
        // This will trigger when any public parameter changes in editor
        SceneView.RepaintAll();
    }

    private void OnDrawGizmosSelected()
    {
        if (tilePrefab == null) return;

        Vector3 centerOffset = autoCenter ?
            new Vector3(
                -(columns - 1) * (tileSize + tileSpacing) / 2f,
                0,
                -(rows - 1) * (tileSize + tileSpacing) / 2f
            ) : Vector3.zero;

        centerOffset += gridOffset;

        // Draw grid outline
        Gizmos.color = Color.green;
        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 position = transform.position + centerOffset +
                    new Vector3(x * (tileSize + tileSpacing), 0, z * (tileSize + tileSpacing));

                // Draw tile outline
                Gizmos.DrawWireCube(position, new Vector3(tileSize, 0.1f, tileSize));

                // Display tile ID
                Handles.Label(position, $"{z}x{x}");
            }
        }
    }
#endif


    public WorldGridTile GetGridTileBase(string id)
    {
        for (int i = 0; i < gridTiles.Count; i++)
        {
            if (gridTiles[i] != null && gridTiles[i].tileId == id)
            {
                return gridTiles[i];
            }
        }
        return null;
    }

    [Button("Generate Grid", ButtonSizes.Large)]
    public void GenerateGrid()
    {
        // First clear existing grid if any
        ClearGrid();

        Transform gridParent = transform;

        if (tilePrefab == null)
        {
            Debug.LogError("Tile prefab is required to generate grid!");
            return;
        }

        Vector3 centerOffset = autoCenter ?
            new Vector3(
                -(columns - 1) * (tileSize + tileSpacing) / 2f,
                0,
                -(rows - 1) * (tileSize + tileSpacing) / 2f
            ) : Vector3.zero;

        centerOffset += gridOffset;

        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 position = centerOffset +
                    new Vector3(x * (tileSize + tileSpacing), 0, z * (tileSize + tileSpacing));

                WorldGridTile tileObject = Instantiate(tilePrefab, gridParent.position + position, Quaternion.identity, gridParent);

                string tileId = $"{z}x{x}";
                tileObject.name = $"Tile_{tileId}";

                tileObject.tileId = tileId;
                tileObject.isOccupied = false;
                //tileObject.plant.worldGridTile = tileObject;
                gridTiles.Add(tileObject);
            }
        }

        Debug.Log($"Generated grid with {rows * columns} tiles ({rows}×{columns})");
        AssignNeigbouringTiles();
    }

    [Button("Clear Grid", ButtonSizes.Medium)]

    public void ClearGrid()
    {
        // Remove all tile GameObjects
        for (int i = gridTiles.Count - 1; i >= 0; i--)
        {
            if (gridTiles[i] != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(gridTiles[i].gameObject);
                }
                else
                {
                    DestroyImmediate(gridTiles[i].gameObject);
                }
            }
        }

        gridTiles.Clear();
        Debug.Log("Grid cleared");
    }

    public Vector3 GetGridTileBasePosition(string id)
    {
        WorldGridTile tile = GetGridTileBase(id);
        return tile != null ? tile.positionalTransform.position : transform.position;
    }

    public void AssignNeigbouringTiles()
    {
        for (int i = 0; i < gridTiles.Count; i++)
        {
            gridTiles[i].GetNeibouringTiles();
        }
    }

    [Button]
    public void ClearOccupiedGridCells(bool useFx = false)
    {
        for (int i = 0; i < gridTiles.Count; i++)
        {
            if (gridTiles[i].isOccupied)
            {
                gridTiles[i].Clear();
            }
        }
    }

    public void ResetTileGrid()
    {
        ClearOccupiedGridCells();

        for (int i = 0; i < gridTiles.Count; i++)
        {
            if (gridTiles[i] != null)
            {
                if(gridTiles[i].tileType != WorldGridTile.TileType.Default)
                {
                    gridTiles[i].SetTile(WorldGridTile.TileType.Default);
                    //gridTiles[i].transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 5, .5f).SetDelay(i * 0.01f);
                }
            }
        }
    }

    public void ResetPlants()
    {
        savedGridTiles.Clear();
        print("clearing plants in grid tiles...");

        for (int i = 0; i < gridTiles.Count; i++)
        {
            if (gridTiles[i] != null)
            {
                gridTiles[i].SetTile(WorldGridTile.TileType.Default);
                gridTiles[i].transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 5, .5f).SetDelay(i * 0.01f);
                gridTiles[i].plant.Reset();
            }
        }
    }

    public void GrowPlantsOnLastRequiredTiles()
    {

        if (savedGridTiles.Count == 0)
        {
            return;
        }

        for (int i = 0; i < savedGridTiles.Count; i++)
        {
            if (savedGridTiles[i] != null)
            {
                savedGridTiles[i].SetTile(WorldGridTile.TileType.Required);
                savedGridTiles[i].plant.Grow();
            }
        }

        savedGridTiles.Clear();

    }


    [Button("Set Required Tiles", ButtonSizes.Medium)]
    public void SetRequiredGridTiles(WordLevelScriptableObject wordLevel)
    {
        List<string> ids = wordLevel.GetRequiredTileIDs();
        for (int i = 0; i < gridTiles.Count; i++)
        {
            if (gridTiles[i] != null)
            {
                if (ids.Contains(gridTiles[i].tileId))
                {
                    gridTiles[i].SetTile(WorldGridTile.TileType.Required);
                    gridTiles[i].plant.Reset();
                    savedGridTiles.Add(gridTiles[i]);
                    gridTiles[i].transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 5, .5f).SetDelay(i * 0.01f);

                }
                else
                {
                    gridTiles[i].SetTile(WorldGridTile.TileType.Default);
                }
            }
        }
    }

    public void  SpawnAlphabetTile(WorldGridTile gridtile, string letter)
    {
        Vector3 spawnPosition = SceneRegistry.Instance.alphabetTileSpawner.GetSpawnPosition().position;
        Vector3 worldPos = gridtile.positionalTransform.position;

        AlphabetTile alphabetTile = SceneRegistry.Instance.alphabetTileSpawner.alphabetTilePool.Get();
        alphabetTile.transform.localScale = Vector3.one * 0.1f;

        SceneRegistry.Instance.alphabetTileSpawner.spawnedAlpabetTiles.Add(alphabetTile);

        alphabetTile.transform.eulerAngles = Vector3.zero;
        alphabetTile.SetLetterMesh(letter);
        alphabetTile.transform.eulerAngles = new Vector3(75, 0, 0);
        alphabetTile.transform.position = spawnPosition;

        Sequence sequence = DOTween.Sequence();

        alphabetTile.transform.DOScale(Vector3.one, .5f);
        sequence
        .Append(alphabetTile.transform.DOMove(worldPos, .8f).SetEase(Ease.OutBounce))
        .Insert(0, alphabetTile.transform.DOLocalRotate(new Vector3(0, 360, 0), sequence.Duration(), RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear))
        .OnComplete
        (
            () =>
            {
                WorldGridTile.TileType tileType = WorldGridTile.TileType.Default;

                //Am destroying duplicate alphabet letter tiles if the tiles has alphabetTile
                if (gridtile.alphabetTile != null)
                {
                    SceneRegistry.Instance.alphabetTileSpawner.alphabetTilePool.Release(alphabetTile);
                    gridtile.alphabetTile.PlayMergeFx();
                    tileType = WorldGridTile.TileType.Merged;
                }
                else
                {
                    tileType = WorldGridTile.TileType.Occupied;
                }

                gridtile.plant.worldGridTile = gridtile;

                gridtile.FireSeedProjectile();

                gridtile.SetTile(tileType);
                gridtile.alphabetTile = alphabetTile;
                alphabetTile.transform.DOPunchScale(Vector3.one * 0.25f, .3f);
            }
        );
    }


    public void TogglePlayerGridTileInteractions(bool interactionEnabled)
    {
        for (int i = 0; i < savedGridTiles.Count; i++)
        {
            if (savedGridTiles[i] != null)
            {
                savedGridTiles[i].SetInteractible(interactionEnabled);
            }
        }
    }


}