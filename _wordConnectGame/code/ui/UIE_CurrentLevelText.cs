using TMPro;
using UnityEngine;

public class UIE_CurrentLevelText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    

    private void OnEnable()
    {
        LevelManager.OnShowMenu += SetLevelText;
        LevelManager.OnLevelManagerInitialized += LevelManager_OnLevelManagerInitialized; ;
        LevelManager.OnLevelManagerComplete += LevelManager_OnLevelManagerComplete;
        LevelManager.OnLevelManagerLoadNextSubLevel += LevelManager_OnLevelManagerLoadNextSubLevel;
    }

    private void LevelManager_OnLevelManagerLoadNextSubLevel(int obj)
    {
        UpdateUIElement();
    }

    private void LevelManager_OnLevelManagerComplete(WordLevelScriptableObject obj)
    {
        levelText.text = string.Empty;
    }

    private void LevelManager_OnLevelManagerInitialized(WordLevelScriptableObject obj)
    {
        UpdateUIElement();
    }

    private void SetLevelText(int level)
    {
        levelText.text = $"Level {(level + 1)} ";
    }

    void UpdateUIElement()
    {
        int i = LevelManager.Instance.levelIndex;
        var levels = LevelManager.Instance.levels;

        if (levels[i].GetSubLevelCount() > 1)
        {
            for (int k = 0; k < levels[i].GetSubLevelCount(); k++)
            {
                levelText.text = $"Level {(i + 1)}-{(LevelManager.Instance.subLevelIndex + 1)}";
            }
        }
        else
        {
            SetLevelText(i);
        }
    }

    private void OnDisable()
    {
        LevelManager.OnShowMenu -= SetLevelText;
        LevelManager.OnLevelManagerInitialized -= LevelManager_OnLevelManagerInitialized; ;
        LevelManager.OnLevelManagerComplete -= LevelManager_OnLevelManagerComplete;
        LevelManager.OnLevelManagerLoadNextSubLevel -= LevelManager_OnLevelManagerLoadNextSubLevel;
    }
}
