using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public bool initializeOnStart;

    [Space]
    public float levelCompletionProgress;

    [Title("Level")]
    public CinemachineCamera topDownCamera;
    public CinemachineCamera menuCamera;
    public CinemachineCamera wordCollectionCam;

    public int levelIndex;
    [ReadOnly] public int subLevelIndex;
    [ReadOnly] public WordLevelScriptableObject selectedWordLevel;
    [ReadOnly] public Level currentLevel;
    [Space] public Level[] levels;

    [Title("Word Parameters")]
    public LevelWordsDictionary levelWordsDictionary;

    [Space]
    public Button startLevelButton;

    [Title("Input tile settings")]
    [InfoBox("USE POOLING", InfoMessageType.Warning)]
    [SerializeField] private float radius = 10;
    [SerializeField] private Transform center;

    [Space]
    [SerializeField] private RectTransform parentRectTransform;
    [SerializeField] private InputLetterTile InputLetterTilePrefab;


    public static event System.Action<int> OnShowMenu;
    public static event System.Action<WordLevelScriptableObject> OnLevelManagerInitialized;
    public static event System.Action<WordLevelScriptableObject> OnLevelManagerComplete;
    public static event System.Action<int> OnLevelManagerLoadNextSubLevel;


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        LevelWordsDictionary.OnLevelWordsDictionaryCompleted += OnLevelComplete;
    }

    void Start()
    {
        if(PlayerPrefs.HasKey("LEVEL_INDEX"))
        {
            levelIndex = PlayerPrefs.GetInt("LEVEL_INDEX");
        }


        ShowMainMenu();
        if (initializeOnStart)
        {
            // Replace Invoke with coroutine
            StartCoroutine(InitializeLevelWithDelay(0.5f));
        }
    }

    public void ShowMainMenu()
    {
        currentLevel = levels[levelIndex];
        selectedWordLevel = currentLevel.wordLevels[subLevelIndex];

        topDownCamera.Prioritize();
        UIDialoguesManager.Instance.ToggleShowMenuHUD(true);
        UIDialoguesManager.Instance.ToggleShowGameHUD(false);

        startLevelButton.onClick.RemoveAllListeners();
        startLevelButton.onClick.AddListener(() => StartLevel());
        OnShowMenu?.Invoke(levelIndex);

        //SceneRegistry.Instance.worldGrid.GetComponent<WorldGridSaveManager>().LoadWorld(SceneRegistry.Instance.worldGrid.gridTiles.ToArray());

        print("ShowMainMenu called, levelIndex: " + levelIndex);
    }

    public void StartLevel()
    {
        topDownCamera.Prioritize();

        UIDialoguesManager.Instance.ToggleShowMenuHUD(false);
        UIDialoguesManager.Instance.ToggleShowGameHUD(true);


        // Initialize the level with a delay
        StartCoroutine(InitializeLevelWithDelay(0.5f));
    }

    // New coroutine method to handle delayed initialization
    private IEnumerator InitializeLevelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitializeLevel();
    }

    public WordLevelScriptableObject GetLevelScriptableObject()
    {
        return selectedWordLevel;
    }


    /// <summary>
    /// Initialize Level; ( level data is derived from WordLevelScriptableObject)
    /// </summary>
    [Button]
    public void InitializeLevel()
    {
        currentLevel = levels[levelIndex];
        selectedWordLevel = currentLevel.wordLevels[subLevelIndex];

        levelWordsDictionary.Initialize(selectedWordLevel);
        SceneRegistry.Instance.worldGrid.SetRequiredGridTiles(selectedWordLevel);
        //SceneRegistry.Instance.worldGrid.ResetTileGrid();
        OnLevelManagerInitialized?.Invoke(selectedWordLevel);
    }


    [Button]
    public void SetLevelIds()
    {
        for (int i = 0; i < levels.Count(); i++)
        {
            if (levels[i].GetSubLevelCount() > 1)
            {
                for (int k = 0; k < levels[i].GetSubLevelCount(); k++)
                {
                    levels[i].levelID = i + "LEVEL" + i + "SUBLEVEL" + k;
                }
            }
            else
            {
                levels[i].levelID = i + "LEVEL" + i;
            }
        }
    }   


    public void OnLevelComplete(LevelCompleteData levelCompleteData)
    {
        if (currentLevel.GetSubLevelCount() > 1)
        {
            if (subLevelIndex < currentLevel.GetSubLevelCount() - 1)
            {
                GetComponent<LevelCompleteHandler>().EndLevel(false);
                return;
            }
            GetComponent<LevelCompleteHandler>().EndLevel(true);
            OnLevelManagerComplete?.Invoke(selectedWordLevel);
            return;
        }

        OnLevelManagerComplete?.Invoke(selectedWordLevel);
    }



    [Button]
    public void LoadNextLevel()
    {
        subLevelIndex = 0;
        levelIndex++;

        PlayerPrefs.SetInt("LEVEL_INDEX", levelIndex);
        PlayerPrefs.Save();

        currentLevel = levels[levelIndex];
        selectedWordLevel = currentLevel.GetCurrendWordLevel();

        UIDialoguesManager.Instance.ToggleShowGameHUD(true);
        topDownCamera.Prioritize();

        InitializeLevel();
    }

    public void LoadNextSubLevel()
    {
        subLevelIndex++;
        selectedWordLevel = currentLevel.wordLevels[subLevelIndex];

        if (selectedWordLevel == null)
        {
            print($"No more sublevels, end the level");
            return;
        }

        SceneRegistry.Instance.worldGrid.GrowPlantsOnLastRequiredTiles();
        OnLevelManagerLoadNextSubLevel?.Invoke(subLevelIndex);

        InitializeLevel();
    }



    private void OnDisable()
    {
        levelWordsDictionary.Cleanup();
        LevelWordsDictionary.OnLevelWordsDictionaryCompleted -= OnLevelComplete;     
    }


}


[System.Serializable]
public class Level
{
    [ReadOnly] public string levelID = "<INT>LEVELINDEX + <INT>SUBLEVEL.COUNT";
    [ReadOnly] public int index;
    public WordLevelScriptableObject[] wordLevels;

    public WordLevelScriptableObject GetCurrendWordLevel()
    {
        return wordLevels[index];
    }

    public WordLevelScriptableObject GetSubLevel(int i)
    {
        return wordLevels[i];
    }


    public int GetSubLevelCount()
    {
        return wordLevels.Length;
    }

    public WordLevelScriptableObject GetCurrendWord(int _index)
    {
        return wordLevels[_index];
    }
}