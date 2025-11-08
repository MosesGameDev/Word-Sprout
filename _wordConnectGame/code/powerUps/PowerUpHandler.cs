using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEditor.PackageManager;
using UnityEngine;

public class PowerUpHandler : MonoBehaviour
{
    [SerializeField] private int hintPowerUpCost = 200;

    private string selectedTileId;

    private void OnEnable()
    {
        WorldGridTile.OnTileClicked += WorldGridTile_OnTileClicked;    
    }

    private void OnDisable()
    {
        WorldGridTile.OnTileClicked -= WorldGridTile_OnTileClicked;
    }

    private void WorldGridTile_OnTileClicked(WorldGridTile obj)
    {
        selectedTileId = obj.tileId;
        SpawnHintLetterTile(obj);
    }

    [Button]
    public void UseHintPowerUp()
    {
        SetCamerFOV(60);
        UIDialoguesManager.Instance.GetUIDialogue("_game").Hide();
        SceneRegistry.Instance.worldGrid.TogglePlayerGridTileInteractions(true);
    }


    public void SetCamerFOV(float targetFOV)
    {
        CinemachineCamera vcam = LevelManager.Instance.topDownCamera;

        float start = vcam.Lens.FieldOfView;

        DOTween.To(
            () => start,
            v =>
            {
                var lens = vcam.Lens;
                lens.FieldOfView = v;
                vcam.Lens = lens;
            },
            targetFOV,
            .6f
        )
        .SetEase(Ease.InOutSine);
    }


    public void SpawnHintLetterTile(WorldGridTile obj)
    {
        SceneRegistry.Instance.worldGrid.TogglePlayerGridTileInteractions(false);
        SetCamerFOV(72f);

        string letter = LevelManager.Instance.currentLevel.GetCurrendWordLevel().GetAlphabetTileLetter(selectedTileId);
        SceneRegistry.Instance.worldGrid.SpawnAlphabetTile(obj, letter);
        UIDialoguesManager.Instance.GetUIDialogue("_game").Show();

        CheckWordCompletion(obj.tileId);
    }


    void CheckWordCompletion(string tileId)
    {
        WordLevelScriptableObject wordLevel = LevelManager.Instance.currentLevel.GetCurrendWordLevel();

        foreach (var word in wordLevel.words)
        {
            bool containsTile = false;

            for (int i = 0; i < word.letters.Length; i++)
            {
                string id = word.letters[i].gridCoords.x + "x" + word.letters[i].gridCoords.y;
                if (id == tileId)
                {
                    containsTile = true;
                    break;
                }
            }

            if (!containsTile)
            {
                continue;
            }

            string completedWord = string.Empty;
            bool allLettersExist = true;

            for (int i = 0; i < word.letters.Length; i++)
            {
                string id = word.letters[i].gridCoords.x + "x" + word.letters[i].gridCoords.y;
                WorldGridTile tile = SceneRegistry.Instance.worldGrid.GetGridTileBase(id);

                if (tile != null && tile.alphabetTile != null)
                {
                    completedWord += tile.alphabetTile.letter;
                }
                else
                {
                    allLettersExist = false;
                    break;
                }
            }

            if (allLettersExist)
            {
                LevelManager.Instance.levelWordsDictionary.ValidatePlayerInput(completedWord);
            }
        }
    }
}
