using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputHandler : MonoBehaviour
{
    [Title("Input tile settings")]
    [InfoBox("USE POOLING", InfoMessageType.Warning)]

    [SerializeField] private float radius = 10;
    [SerializeField] private Transform center;

    [Space]
    [SerializeField] private RectTransform shuffleButton;
    [SerializeField] private RectTransform inputButtonBackground;
    [SerializeField] private RectTransform parentRectTransform;
    [SerializeField] private InputLetterTile InputLetterTilePrefab;

    [Space]
    public List<InputLetterTile> spawnedInputLetterTiles = new List<InputLetterTile>();


    private void OnEnable()
    {
        LevelManager.OnLevelManagerInitialized += LevelManager_OnLevelManagerInitialized;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputValid += ClearValidPlayerInput;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputInvalid += ClearInvalidPlayerInput;
    }

    private void LevelManager_OnLevelManagerInitialized(WordLevelScriptableObject obj)
    {
        ShowInput(obj);
    }

    private void OnDisable()
    {
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputValid -= ClearValidPlayerInput;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputInvalid -= ClearInvalidPlayerInput;
        LevelManager.OnLevelManagerInitialized -= LevelManager_OnLevelManagerInitialized;

    }


    private void Start()
    {
        shuffleButton.localScale = Vector3.zero;
        inputButtonBackground.localScale = Vector3.zero;
    }

    public void ShowInput(WordLevelScriptableObject obj)
    {
        Sequence showInputSequence = DOTween.Sequence();

        showInputSequence.Append(inputButtonBackground.DOScale(Vector3.one, .7f).SetEase(Ease.OutBack))
            .Join(shuffleButton.DOScale(Vector3.one, 0.3f).SetDelay(0.15f).SetEase(Ease.OutBounce));

        showInputSequence.OnComplete( () => {
            SpawnInputLetterTiles(obj);
        });
    }


    void SpawnInputLetterTiles(WordLevelScriptableObject selectedWordLevel)
    {

        if(spawnedInputLetterTiles.Count > 0)
        {
            ClearInputLetterTiles();
        }

        string[] letters = selectedWordLevel.letters;
        Vector2 middlePosition = parentRectTransform.anchoredPosition;

        for (int i = 0; i < letters.Length; i++)
        {
            float angle = 2f * Mathf.PI * i / letters.Length;
            float x = center.position.x + radius * Mathf.Cos(angle);
            float y = center.position.y + radius * Mathf.Sin(angle);

            Vector3 spawnPosition = new Vector3(x, y, center.position.z);


            InputLetterTile letterTile = Instantiate(InputLetterTilePrefab, parentRectTransform);

            letterTile.GetComponent<RectTransform>().localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();

            sequence
                .Append(letterTile.GetComponent<RectTransform>().DOMove(spawnPosition, 0.3f))
                .Insert(0, letterTile.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack))
                .SetDelay(i * 0.1f);

            letterTile.GetComponent<RectTransform>().SetParent(parentRectTransform.parent);
            letterTile.SetLetterText(letters[i]);
            spawnedInputLetterTiles.Add(letterTile);
        }
    }


    public void ClearInputLetterTiles()
    {
        for (int i = 0; i < spawnedInputLetterTiles.Count; i++)
        {
            if(spawnedInputLetterTiles[i].gameObject != null)
            {
                Destroy(spawnedInputLetterTiles[i].gameObject);
            }
        }

        spawnedInputLetterTiles.Clear();
    }


    public void ClearValidPlayerInput(string input)
    {
        DeselectInputTiles(false);
    }


    public void ClearInvalidPlayerInput(string input)
    {
        DeselectInputTiles(true);
    }


    void DeselectInputTiles(bool playShake)
    {
        for (int i = 0; i < spawnedInputLetterTiles.Count; i++)
        {
            if (playShake)
            {
                spawnedInputLetterTiles[i].PlayShakeTween(.3f, playShake);
            }
            spawnedInputLetterTiles[i].SetDeselected();
        }
    }

}
