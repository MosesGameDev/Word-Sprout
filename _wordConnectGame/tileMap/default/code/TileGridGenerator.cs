using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UIElements;

public class TileGridGenerator : MonoBehaviour
{
    [BoxGroup("Grid Size")]
    public int X, Y;

    [Space]
    [SerializeField] private bool showDebugText;

    [Space]
    public Tilemap tilemap;
    public RuleTile tileRule;

    [Space]
    public TextMeshPro debugTextPrefab;
    public ParticleSystem clearBaseTileFxPrefab;

    public List<GridTileBase> dynamicGridTiles = new List<GridTileBase>();


    public GridTileBase GetGridTileBase(string id)
    {
        for (int i = 0; i < dynamicGridTiles.Count; i++)
        {
            if (dynamicGridTiles[i].tileId == id)
            {
                return dynamicGridTiles[i];
            }
        }

        return null;
    }

    public Vector3 GetGridTileBasePosition(string id)
    {
        return tilemap.GetCellCenterWorld(GetGridTileBase(id).tilePosition);
    }

    public void GenerateGrid()
    {
        if(dynamicGridTiles.Count > 0)
        {
            dynamicGridTiles.Clear();
        }

        for (int x = 0; x < X; x++)
        {
            for (int y = 0; y < Y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tileRule);

                Vector3 pos = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                string id = y + "x" + x;
                GridTileBase tile = new GridTileBase(id, new Vector3Int(x, y, 0));
                dynamicGridTiles.Add(tile);

                if (showDebugText)
                {
                    TextMeshPro textMesh = Instantiate(debugTextPrefab, pos, new Quaternion());
                    textMesh.SetText(id);
                }
            }
        }
    }


    /// <summary>
    /// Clears tiles that are not occupied by alphabet 3d tile
    /// </summary>
    [Button]
    public void ClearUnOccupiedGridCells(bool useFx=false)
    {
    }

    public void SetTileNull(Vector3Int tilePosition)
    {
        Vector3 pos = tilemap.GetCellCenterWorld(tilePosition);

        ParticlePool.Instance.SpawnFromPool("tileDestroyFx", pos, new Quaternion());
        tilemap.SetTile(tilePosition, null);
    }


    public void DestroyNeighboringTiles(Vector3Int position)
    {
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
        new Vector3Int(0, 1, 0),  // North
        new Vector3Int(1, 1, 0),  // Northeast
        new Vector3Int(1, 0, 0),  // East
        new Vector3Int(1, -1, 0), // Southeast
        new Vector3Int(0, -1, 0), // South
        new Vector3Int(-1, -1, 0),// Southwest
        new Vector3Int(-1, 0, 0), // West
        new Vector3Int(-1, 1, 0)  // Northwest  
        };

        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPos = position + offset;
            TileBase neighborTile = tilemap.GetTile(neighborPos);

            if (neighborTile != null)
            {
                SetTileNull(neighborPos);
            }
        }
    }

    [Button]
    public void ResetTileGrid()
    {
        GenerateGrid();
    }


}


[System.Serializable]
public class GridTileBase
{
    public string tileId;
    public Vector3Int tilePosition;
    public TileBase tile;
    [Space]
    public bool isOccupied;
    [Space]
    public AlphabetTile alphabetTile;

    public GridTileBase(string tileId, Vector3Int tilePosition)
    {
        this.tileId = tileId;
        this.tilePosition = tilePosition;
    }
}