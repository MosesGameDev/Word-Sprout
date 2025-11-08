using UnityEngine;
using UnityEngine.UI;

public class UIE_SubLevelIndicator : MonoBehaviour
{
    [SerializeField] private Image[] markers;

    private void OnEnable()
    {
        LevelManager.OnLevelManagerInitialized += LevelManager_OnLevelManagerInitialized;
        LevelManager.OnLevelManagerComplete += LevelManager_OnLevelManagerComplete;
        LevelManager.OnLevelManagerLoadNextSubLevel += LevelManager_OnLevelManagerLoadNextSubLevel;
    }

    private void LevelManager_OnLevelManagerComplete(WordLevelScriptableObject obj)
    {
        for (int i = 0; i < markers.Length; i++)
        {
            markers[i].color = Color.gray;
            markers[i].gameObject.SetActive(false);
        }
    }

    private void LevelManager_OnLevelManagerInitialized(WordLevelScriptableObject obj)
    {
        if(LevelManager.Instance.subLevelIndex != 0)
        {
            return;
        }

        int subLevels = LevelManager.Instance.currentLevel.GetSubLevelCount();

        for (int i = 0; i < subLevels; i++)
        {
            markers[i].gameObject.SetActive(true);
            markers[i].color = Color.gray;
        }

        markers[0].color = Color.green;

    }

    private void LevelManager_OnLevelManagerLoadNextSubLevel(int _sublevelIndex)
    {
        UpdateUIElement(_sublevelIndex);
    }

    private void UpdateUIElement(int _sublevelIndex)
    {
        
        int subLevels = LevelManager.Instance.currentLevel.GetSubLevelCount();
        print($"SUBLEVELS: {subLevels}. CURRENT INDEX: {_sublevelIndex}");

        for (int i = 0; i < markers.Length; i++)
        {
            markers[i].color = Color.gray;
        }

        markers[_sublevelIndex].color = Color.green;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelManagerInitialized -= LevelManager_OnLevelManagerInitialized;
        LevelManager.OnLevelManagerLoadNextSubLevel -= LevelManager_OnLevelManagerLoadNextSubLevel;
        LevelManager.OnLevelManagerComplete -= LevelManager_OnLevelManagerComplete;

    }

}
