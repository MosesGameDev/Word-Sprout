using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;


public class LevelCompleteData
{
    public float levelProgress;
    public List<string> discoveredWords = new List<string>();

    public LevelCompleteData(float _levelProgress, List<string> _discoveredWords)
    {
        levelProgress = _levelProgress;
        discoveredWords = _discoveredWords;
    }

}


[System.Serializable]
public class LevelWordsDictionary
{
    public float levelProgress;

    int discoveredRequiredWords;


    [Space]
    [SerializeField] private List<string> words = new List<string>();

    [Space]
    public List<string> requiredWordsList = new List<string>();
    public List<string> bonusWordsList = new List<string>();

    [Space]
    public List<string> discoveredWords = new List<string>();

    public static event System.Action<string> OnLevelWordsDictionaryPlayerInputValid;
    public static event System.Action<string> OnLevelWordsDictionaryPlayerInputInvalid;
    public static event System.Action<LevelCompleteData> OnLevelWordsDictionaryCompleted;


    /// <summary>
    /// Assign words to lists from WordLevelScriptableObject
    /// </summary>
    /// <param name="wordLevel_SO"></param>
    public void Initialize(WordLevelScriptableObject wordLevel_SO)
    {
        // Clear previous data when re-initializing
        words.Clear();
        requiredWordsList.Clear();
        bonusWordsList.Clear();
        discoveredWords.Clear();
        discoveredRequiredWords = 0;

        LevelInputManager.OnLevelInputManagerValidatePlayerInput -= ValidatePlayerInput;
        LevelInputManager.OnLevelInputManagerValidatePlayerInput += ValidatePlayerInput;

        for (int i = 0; i < wordLevel_SO.words.Length; i++)
        {
            if (wordLevel_SO.words[i].isRequired)
            {
                requiredWordsList.Add(wordLevel_SO.words[i].word);
            }
            else
            {
                bonusWordsList.Add(wordLevel_SO.words[i].word);
            }
        }

        // Add all words to the main words list
        for (int i = 0; i < requiredWordsList.Count; i++)
        {
            words.Add(requiredWordsList[i]);
        }

        if (bonusWordsList.Count > 0)
        {
            for (int i = 0; i < bonusWordsList.Count; i++)
            {
                words.Add(bonusWordsList[i]);
            }
        }

        Debug.Log($"Initialized with {requiredWordsList.Count} required words and {bonusWordsList.Count} bonus words");

    }


    /// <summary>
    /// Check if player input value can be found in word lists
    /// </summary>
    /// <param name="input"></param>
    public void ValidatePlayerInput(string input)
    {

        if (words.Contains(input))
        {
            OnLevelWordsDictionaryPlayerInputValid?.Invoke(input);

            words.Remove(input);
            discoveredWords.Add(input);

            // Check if it's a bonus word or required word
            if (bonusWordsList.Contains(input))
            {
                Debug.Log("<color=blue><b>Bonus Word found: </b> </color>" + input);
                // Don't increment discoveredRequiredWords for bonus words
            }
            else if (requiredWordsList.Contains(input))
            {
                Debug.Log("<color=green><b>Required Word found: </b> </color>" + input);
                discoveredRequiredWords++;
                CalculateLevelProgress();
            }

            return;
        }

        if (discoveredWords.Contains(input))
        {
            Debug.Log("<color=yellow><b>Word already found:</b> </color>" + input);
        }


        OnLevelWordsDictionaryPlayerInputInvalid?.Invoke(input);
    }

    /// <summary>
    /// Returns true if word is valid (in level dictionary)
    /// </summary>
    public bool IsInputValid(string input)
    {
        bool b = false;

        if (words.Contains(input))
        {
            b = true;
        }

        return b;
    }

    void CalculateLevelProgress()
    {
        float requiredWords = discoveredRequiredWords;
        float requiredWordstotal = requiredWordsList.Count;
        levelProgress = requiredWords / requiredWordstotal;

        Debug.Log($"Level progress: {levelProgress} ({discoveredRequiredWords}/{requiredWordsList.Count})");

        if (levelProgress >= 1)
        {
            LevelCompleteData levelCompleteData = new LevelCompleteData(levelProgress, discoveredWords);
            OnLevelWordsDictionaryCompleted?.Invoke(levelCompleteData);
        }
    }

    public void Cleanup()
    {
        LevelInputManager.OnLevelInputManagerValidatePlayerInput -= ValidatePlayerInput;
    }


}