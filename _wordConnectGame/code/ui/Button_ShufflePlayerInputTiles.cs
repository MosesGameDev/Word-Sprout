using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;


[RequireComponent(typeof(Button))]
public class Button_ShufflePlayerInputTiles : MonoBehaviour
{
    public int numberOfShuffles = 10;
    private float coolDownTime = .3f;
    [Space]
    [SerializeField] private PlayerInputHandler inputHandler;

    private void OnEnable()
    {
        LevelManager.OnLevelManagerInitialized += LevelManager_OnLevelManagerInitialized;
        LevelManager.OnLevelManagerComplete += LevelManager_OnLevelManagerComplete;
    }


    private void OnDisable()
    {
        LevelManager.OnLevelManagerInitialized -= LevelManager_OnLevelManagerInitialized;
        LevelManager.OnLevelManagerComplete -= LevelManager_OnLevelManagerComplete;

    }

    private void LevelManager_OnLevelManagerInitialized(WordLevelScriptableObject obj)
    {
        if (numberOfShuffles <= 0)
        {
            GetComponent<Button>().interactable = false;
            return;
        }

        GetComponent<Button>().interactable = true;
    }

    private void LevelManager_OnLevelManagerComplete(WordLevelScriptableObject obj)
    {
        GetComponent<Button>().interactable = false;
    }


    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { ShuffleInputTilesPositions(); });
    }



    public void ShuffleInputTilesPositions()
    {
        if(numberOfShuffles <= 0)
        {
            print("OUT OF SHUFFLES");
            return;
        }

        GetComponent<Button>().interactable = false;
        StartCoroutine(ShuffleEnum());
    }


    public void ResetReshuffles(int reshuffleCount=10)
    {
        numberOfShuffles += reshuffleCount;
        GetComponent<Button>().interactable = true;
    }


    IEnumerator ShuffleEnum()
    {

        List<InputLetterTile> spawnedInputLetterTiles = inputHandler.spawnedInputLetterTiles;

        List<Vector2> positions = new List<Vector2>();

        for (int i = 0; i < spawnedInputLetterTiles.Count; i++)
        {
            Vector2 v = spawnedInputLetterTiles[i].GetComponent<RectTransform>().position;
            positions.Add(v);
        }

        positions.Shuffle();

        for (int i = 0; i < spawnedInputLetterTiles.Count; i++)
        {
            spawnedInputLetterTiles[i].GetComponent<RectTransform>().DOMove(positions[i], 0.3f);
        }

        numberOfShuffles--;

        yield return new WaitForSeconds(coolDownTime);

        GetComponent<Button>().interactable = true;
    }
}
