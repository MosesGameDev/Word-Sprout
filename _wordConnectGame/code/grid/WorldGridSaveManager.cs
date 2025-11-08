using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldGridSaveManager : MonoBehaviour
{
    [System.Serializable]
    public class TileData
    {
        public string tileId;
        public bool isOccupied;
        public WorldGridTile.TileType tileType;
        public int plantLevel;
        public PlantObject.plantType plantType;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<TileData> tiles = new List<TileData>();
    }

    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "world_save.json");
    }

    private void OnEnable()
    {
        LevelManager.OnLevelManagerComplete += LevelManager_OnLevelManagerComplete;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelManagerComplete -= LevelManager_OnLevelManagerComplete;
    }

    private void LevelManager_OnLevelManagerComplete(WordLevelScriptableObject obj)
    {
        print("<color=#7beb31><b>Level completed, saving world state...</b></color>");
        WorldGridTile[] tiles = SceneRegistry.Instance.worldGrid.gridTiles.ToArray();
        SaveWorld(tiles);
    }

    public void SaveWorld(WorldGridTile[] tiles)
    {
        SaveData saveData = new SaveData();

        foreach (var tile in tiles)
        {
            if (tile == null) continue;

            TileData data = new TileData
            {
                tileId = tile.tileId,
                isOccupied = tile.isOccupied,
                tileType = tile.tileType,
                plantLevel = tile.plant != null ? tile.plant.currentLevel : 0,
                plantType = tile.plant != null ? tile.plant.type : PlantObject.plantType.Corn
            };
            saveData.tiles.Add(data);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("World saved to " + savePath);
    }

    public void LoadWorld(WorldGridTile[] tiles)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Save file not found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        foreach (var savedTile in saveData.tiles)
        {
            WorldGridTile tile = System.Array.Find(tiles, t => t.tileId == savedTile.tileId);
            if (tile != null)
            {
                tile.isOccupied = savedTile.isOccupied;
                tile.SetTile(savedTile.tileType);

                if (tile.plant != null)
                {
                    tile.plant.type = savedTile.plantType;
                    tile.plant.currentLevel = savedTile.plantLevel;
                    tile.plant.Grow(); // To update visuals
                }
            }
        }

        Debug.Log("World loaded from " + savePath);
    }
}
