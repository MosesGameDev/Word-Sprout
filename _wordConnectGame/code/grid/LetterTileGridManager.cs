using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterTileGridManager : MonoBehaviour
{
    public int index;
    [SerializeField] private TileGridGenerator gridPrefab;
    [SerializeField] private TileGridGenerator grid;
    [Space]
    [SerializeField] private LevelManager levelManager;



    private void OnEnable()
    {
        LevelManager.OnLevelManagerInitialized += LevelManager_OnLevelManagerInitialized;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelManagerInitialized -= LevelManager_OnLevelManagerInitialized;
    }

    private void LevelManager_OnLevelManagerInitialized(WordLevelScriptableObject @object)
    {
        if(grid != null)
        {
            GetCurrentGrid().ResetTileGrid();
            return;
        }

        CreateGrid();
    }


    [Button]
    private void CreateGrid()
    {
        grid = Instantiate(gridPrefab, transform);
        grid.GenerateGrid();
    }


    public TileGridGenerator GetCurrentGrid()
    {
        return grid;
    }




}
