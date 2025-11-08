using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using NUnit.Framework;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;
using System;

public class PlayerInputDisplay : SerializedMonoBehaviour
{
    [SerializeField] DisplayLetterTile letterTilePrefab;

    [SerializeField] private Dictionary<InputLetterTile, DisplayLetterTile> letterTiles = new Dictionary<InputLetterTile, DisplayLetterTile>();

    RectTransform rectTransform;
    string input;

    [Space]
    [SerializeField] private List<DisplayLetterTile> spawnedDisplayTilesList = new List<DisplayLetterTile>();

    public static event Action OnPlayerInputDisplayValidInput;
    public static event Action<List<DisplayLetterTile>, string> OnFindBonusWord;


    private void OnEnable()
    {
        InputLetterTile.OnInputLetterTileSelected += AddLetterTile;
        InputLetterTile.OnInputLetterTileDeselected += RemoveLetterTile;

        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputValid += OnPlayerInputValid;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Used OnInputLetterTileSelected event to add displaytiles
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="rect"></param>
    void AddLetterTile(string letter, RectTransform rect)
    {
        DisplayLetterTile tile = Instantiate(letterTilePrefab, rectTransform);

        tile.GetComponent<RectTransform>().localScale = Vector3.one * .5f;
        tile.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);

        tile.SetText(letter);

        //Dictionary<Player input tile, display tile>
        letterTiles.Add(rect.GetComponent<InputLetterTile>(), tile);
        spawnedDisplayTilesList.Add(tile);

        if (GetComponent<CanvasGroup>().alpha == 0)
        {
            GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        }
    }

    /// <summary>
    /// Used OnInputLetterTileSelected event to remove displaytiles
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="rect"></param>
    void RemoveLetterTile(string letter, RectTransform rect)
    {
        InputLetterTile refrenceTile = rect.GetComponent<InputLetterTile>();
        letterTiles.TryGetValue(refrenceTile, out DisplayLetterTile tile);

        if (tile != null)
        {
            tile.DestroyTile(Color.red);
            spawnedDisplayTilesList.Remove(tile);
            letterTiles.Remove(refrenceTile);

            if (spawnedDisplayTilesList.Count < 1)
            {
                GetComponent<CanvasGroup>().DOFade(0, 0.2f);
            }
        }
    }

    /// <summary>
    /// Destroy spawned letters, clear Dictionary
    /// </summary>
    void ClearDisplayedLetters()
    {
        DisplayLetterTile[] tiles = letterTiles.Values.ToArray();

        for (int i = 0; i < tiles.Length; i++)
        {
            DisplayLetterTile _t = tiles[i];
            _t.DestroyTile(Color.red);
        }

        letterTiles.Clear();
    }

    void OnPlayerInputValid(string _input)
    {
        input = _input;

        // Check if the word is a bonus word
        bool isBonus = IsBonusWord(input);

        if (isBonus)
        {
            // Handle bonus words differently - just clear the tiles with a special effect
            HandleBonusWord(_input);
            return;
        }
        else
        {
            // Normal (required) word processing
            AssignGridIdsToDisplayTiles(input);
            MoveDisplayTiles();
        }

        Clear();

        OnPlayerInputDisplayValidInput?.Invoke();
    }


    /// <summary>
    /// Check if the word is a bonus word by checking its grid coordinates
    /// </summary>
    private bool IsBonusWord(string word)
    {
        var so = LevelManager.Instance.GetLevelScriptableObject();

        for (int i = 0; i < so.words.Length; i++)
        {
            if (so.words[i].word == word)
            {
                // Check if this is a bonus word (isRequired == false or first letter has invalid grid coords)
                if (!so.words[i].isRequired ||
                    (so.words[i].letters.Length > 0 &&
                     (so.words[i].letters[0].gridCoords.x >= 99 || so.words[i].letters[0].gridCoords.y >= 99)))
                {
                    return true;
                }
                return false;
            }
        }
        return false; // Default to false if word not found
    }


    /// <summary>
    /// Handle bonus words with a special effect instead of grid movement
    /// </summary>
    private void HandleBonusWord(string word)
    {
        List<DisplayLetterTile> _spawnedDisplayTilesList = spawnedDisplayTilesList;
        OnFindBonusWord?.Invoke(_spawnedDisplayTilesList, word);
        Clear();
    }

    public void Clear()
    {
        SceneRegistry.Instance.levelInputManager.playerInput = string.Empty;
        spawnedDisplayTilesList.Clear();
        letterTiles.Clear();
        GetComponent<CanvasGroup>().DOFade(0, 0.2f);

    }


    /// <summary>
    /// Assign grid Ids to display tiles
    /// </summary>
    void AssignGridIdsToDisplayTiles(string input)
    {
        var so = LevelManager.Instance.GetLevelScriptableObject();

        // Find the word definition for the input
        WordObject wordObj = null;
        for (int i = 0; i < so.words.Length; i++)
        {
            if (so.words[i].word == input)
            {
                wordObj = so.words[i];
                break;
            }
        }

        if (wordObj == null)
            return;

        // Create a copy of the display tiles list to track which ones are already assigned
        List<DisplayLetterTile> availableTiles = new List<DisplayLetterTile>(spawnedDisplayTilesList);

        // For each letter in the word definition
        for (int k = 0; k < wordObj.letters.Length; k++)
        {
            string currentLetter = wordObj.letters[k].letter;
            Vector2 targetCoords = wordObj.letters[k].gridCoords;
            string targetTileId = targetCoords.x + "x" + targetCoords.y;

            // Find a matching available tile
            DisplayLetterTile matchingTile = null;
            for (int j = 0; j < availableTiles.Count; j++)
            {
                if (availableTiles[j].value == currentLetter)
                {
                    matchingTile = availableTiles[j];
                    break;
                }
            }

            if (matchingTile != null)
            {
                // Assign the target ID
                matchingTile.targetTileId = targetTileId;
                // Remove the tile from available tiles so it's not used again
                availableTiles.Remove(matchingTile);
            }
        }
    }


    /// <summary>
    /// Move each of the spawned display tiles to grid
    /// </summary>
    void MoveDisplayTiles()
    {
        //var gridManager = SceneRegistry.Instance.tileGridManager;
        var worldGrid = SceneRegistry.Instance.worldGrid;

        for (int i = 0; i < spawnedDisplayTilesList.Count; i++)
        {
            if (string.IsNullOrEmpty(spawnedDisplayTilesList[i].targetTileId) ||
                spawnedDisplayTilesList[i].targetTileId.Contains("99x99"))
            {
                // Skip tiles with invalid target IDs
                return;
            }

            var worldTile = worldGrid.GetGridTileBase(spawnedDisplayTilesList[i].targetTileId);

            // Check if the grid tile exists
            if (worldTile != null)
            {
               // Vector3Int tilePos = gridTile.tilePosition;

                spawnedDisplayTilesList[i].targetWorldGridTile = worldTile;
                worldTile.isOccupied = true;

                spawnedDisplayTilesList[i].MoveAlphabetToTargetTile(worldTile, 0.1f * i);
            }
        }
    }

    private void OnDisable()
    {
        InputLetterTile.OnInputLetterTileSelected -= AddLetterTile;
        InputLetterTile.OnInputLetterTileDeselected -= RemoveLetterTile;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputValid -= OnPlayerInputValid;
    }
}