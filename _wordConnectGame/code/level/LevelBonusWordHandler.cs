using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LevelBonusWordHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bonusWordText;
    [SerializeField] private BonusWordUIE bonusWordUIE_Prefab;
    [SerializeField] private Button button;
    [Space]
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private Slider discoveredWordProgreesSlider;

    [Space]
    [SerializeField] float bonusWordsDiscovered;

    List<BonusWordUIE> spawnedBonusWordUIEs = new List<BonusWordUIE>();

    int bonusWordsFound;
    int totalBonusWordsRequired = 5;
    RectTransform letterTileMoveToTarget;

    private void OnEnable()
    {
        PlayerInputDisplay.OnFindBonusWord += PlayerInputDisplay_OnFindBonusWord;
        LevelManager.OnLevelManagerInitialized += LevelManager_OnLevelManagerInitialized;
        LevelManager.OnLevelManagerComplete += LevelManager_OnLevelManagerComplete;
    }


    private void OnDisable()
    {
        LevelManager.OnLevelManagerInitialized -= LevelManager_OnLevelManagerInitialized;
        PlayerInputDisplay.OnFindBonusWord -= PlayerInputDisplay_OnFindBonusWord;
        LevelManager.OnLevelManagerComplete -= LevelManager_OnLevelManagerComplete;

    }

    private void LevelManager_OnLevelManagerInitialized(WordLevelScriptableObject obj)
    {
        button.interactable = true;
    }

    private void LevelManager_OnLevelManagerComplete(WordLevelScriptableObject obj)
    {
        button.interactable = false;
    }


    private void PlayerInputDisplay_OnFindBonusWord(List<DisplayLetterTile> spawnedTiles, string word)
    {
        if (letterTileMoveToTarget == null)
        {
            letterTileMoveToTarget = button.GetComponent<RectTransform>();
        }

        StartCoroutine(OnBonusWordDiscoveredCoroutine(spawnedTiles, word));
    }

    private IEnumerator OnBonusWordDiscoveredCoroutine(List<DisplayLetterTile> spawnedTiles, string word)
    {
        List<Tween> allTweens = new List<Tween>();

        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            RectTransform tileRect = spawnedTiles[i].GetComponent<RectTransform>();
            Vector3 worldPosition = tileRect.position;
            spawnedTiles[i].transform.SetParent(transform, false);
            tileRect.anchorMin = Vector2.zero;
            tileRect.anchorMax = Vector2.zero;
            tileRect.position = worldPosition;

            // Create the tween and store it in our list
            Tween tween = tileRect.DOAnchorPos(letterTileMoveToTarget.anchoredPosition, 0.5f)
            .SetDelay(i * 0.15f)
            .OnComplete
            (
                () => 
                {
                    letterTileMoveToTarget.DOKill();
                    letterTileMoveToTarget.localScale = Vector3.one;
                    letterTileMoveToTarget.DOPunchScale(Vector3.one * 0.1f, 0.5f);
                    Destroy(tileRect.gameObject); 
                }
            );

            allTweens.Add(tween);
        }

        // Wait for all tweens to complete
        foreach (var tween in allTweens)
        {
            yield return tween.WaitForCompletion();
        }

        CreatBonusWordUIE(word);
        OnBonusWordDiscovered();
    }

    void CreatBonusWordUIE(string word)
    {
        BonusWordUIE bonusWordUIE = Instantiate(bonusWordUIE_Prefab, contentTransform);
        bonusWordUIE.SetElementText(word);
        spawnedBonusWordUIEs.Add(bonusWordUIE);
    }


    void ClearFoundBonusWordUIElements()
    {
        for (int i = 0; i < spawnedBonusWordUIEs.Count; i++)
        {
            Destroy(spawnedBonusWordUIEs[i].gameObject);
        }
    }


    void OnBonusWordDiscovered()
    {
        if(bonusWordsFound < totalBonusWordsRequired)
        {
            bonusWordsFound++;

            if(bonusWordsFound >= totalBonusWordsRequired)
            {
                ShowReward();
                return;
            }

            float _bonusWordsFound = bonusWordsFound;
            bonusWordsDiscovered = _bonusWordsFound/totalBonusWordsRequired;
            bonusWordText.SetText("" + bonusWordsFound + "/" + totalBonusWordsRequired);
            discoveredWordProgreesSlider.DOValue(bonusWordsDiscovered, 0.5f);
            return;
        }
    }


    void ShowReward()
    {
        print("SHOW REWARD");
        bonusWordsDiscovered = 0;
        bonusWordsFound = 0;
        discoveredWordProgreesSlider.DOValue(0, 0.5f);
        totalBonusWordsRequired += 10;
    }




}
