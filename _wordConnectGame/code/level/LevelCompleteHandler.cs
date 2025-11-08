using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class LevelCompleteHandler : MonoBehaviour
{

    public GameObject collectionCrate;

    private void OnEnable()
    {
        LevelManager.OnLevelManagerComplete += LevelManager_OnLevelManagerComplete;
        AlphabetTileTweenHandler.OnTweenCollectionFXComplete += AlphabetTileTweenHandler_OnTweenCollectionFXComplete;
    }

    Tween punchTween;
    private void AlphabetTileTweenHandler_OnTweenCollectionFXComplete()
    {
        if (punchTween != null && punchTween.IsActive())
        {
            return;
        }


    }

    private void LevelManager_OnLevelManagerComplete(WordLevelScriptableObject obj)
    {
        EndLevel(true);
    }

    private void OnDisable()
    {
        LevelManager.OnLevelManagerComplete -= LevelManager_OnLevelManagerComplete;
        AlphabetTileTweenHandler.OnTweenCollectionFXComplete -= AlphabetTileTweenHandler_OnTweenCollectionFXComplete;
    }

    [Button]
    public void EndLevel(bool _hasSublevels)
    {
        StartCoroutine(EndLevelEnum(_hasSublevels));
    }


    IEnumerator EndLevelEnum(bool _hasSublevels)
    {
        yield return new WaitForSeconds(1f);
        SceneRegistry.Instance.playerInputHandler.ClearInputLetterTiles();

        yield return new WaitForSeconds(1.5f);
        SceneRegistry.Instance.alphabetTileSpawner.PlayAlphabetTilesCollectionFx();

        ///Move to new method
        SceneRegistry.Instance.worldGrid.ResetTileGrid();

        yield return new WaitForSeconds(1f);
        SceneRegistry.Instance.alphabetTileSpawner.spawnedAlpabetTiles.Clear();

        yield return new WaitForSeconds(2f);

        if (_hasSublevels)
        {
            UIDialoguesManager.Instance.GetUIDialogue("levelComplete").Show();
            UIDialoguesManager.Instance.ToggleShowGameHUD(false);
            LevelManager.Instance.menuCamera.Prioritize();
            //SceneRegistry.Instance.worldGrid.GrowPlantsOnLastRequiredTiles();

        }
        else
        {
            LevelManager.Instance.LoadNextSubLevel();
        }
    }

    void OnAlphabetTilesCollected()
    {

    }



}
