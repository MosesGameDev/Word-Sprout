using System.Collections;
using TMPro;
using UnityEngine;

public class InputResponseTextFx : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI responseText;
    [SerializeField] private UIDialogueElement dialogueElement;

    [Space]
    [SerializeField] private float autoHideDelay = 2f;

    [Space]
    [SerializeField] private string[] responses;
    [Space]
    [SerializeField] private string[] colorHexCodes;
    int index = 0;

    private void OnEnable()
    {
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputValid += LevelWordsDictionary_OnLevelWordsDictionaryPlayerInputValid;
    }

    private void LevelWordsDictionary_OnLevelWordsDictionaryPlayerInputValid(string obj)
    {
        ShowText();
    }

    void ShowText()
    {
        responseText.text = responses[index];
        SetRandomColorSetRandomColor();

        if (dialogueElement == null)
        {
            return;
        }
        dialogueElement.Show();

        if (index < responses.Length - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        StartCoroutine(AutoHide());
    }


    private void SetRandomColorSetRandomColor()
    {
        if (colorHexCodes.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, colorHexCodes.Length);
        string hexColor = "#" + colorHexCodes[randomIndex];
        Color color;

        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            responseText.color = color;
        }
        else
        {
            Debug.LogWarning("Invalid color hex code: " + hexColor);
        }
    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(autoHideDelay);
        dialogueElement.Hide();
    }

    private void OnDisable()
    {
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputInvalid -= LevelWordsDictionary_OnLevelWordsDictionaryPlayerInputValid;
    }
}
