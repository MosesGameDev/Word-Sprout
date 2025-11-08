using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WordObject
{
    public string word;

    [Space]
    public bool isRequired;
    public int score = 1;

    [Space]
    public LetterObject[] letters;


}
