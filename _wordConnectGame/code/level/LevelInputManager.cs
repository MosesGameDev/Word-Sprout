using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI.Extensions;

public class LevelInputManager : MonoBehaviour
{

    public bool initialized;
    public bool inputInitialized;

    [SerializeField] private Vector2 mousePosition;

    [Space]
    public string playerInput;

    [Space]
    public Canvas canvas;
    [SerializeField] private RectTransform rectPointer;
    

    public static event System.Action<string> OnLevelInputManagerValidatePlayerInput;
    public static event System.Action OnLevelInputManagerCorrectInput;



    private void OnEnable()
    {
        InputLetterTile.OnInputLetterTileSelected += AddPlayerInput;
        InputLetterTile.OnInputLetterTileDeselected += RemovePlayerInput;

        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputInvalid += ClearPlayerInput;
    }

    public RectTransform GetPointerRect()
    {
        return rectPointer;
    }


    private void Update()
    {
        if (initialized)
        {
            if(Input.GetMouseButtonDown(0))
            {
                rectPointer.DOKill(true);
                rectPointer.DOScale(Vector3.one, 0.3f);
                inputInitialized = true;
            }

            mousePosition = Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
            {
                rectPointer.DOKill(true);
                rectPointer.DOScale(Vector3.zero, 0.3f);
                inputInitialized = false;
                if (playerInput.Length > 0)
                {
                    ValidatePlayerInput();
                    return;
                }
            }

            UpdateRectPointer();
        }
    }


    void UpdateRectPointer()
    {
        if (rectPointer == null)
        {
            return;
        }

        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out movePos
        );

        rectPointer.position = canvas.transform.TransformPoint(movePos);
    }

    /// <summary>
    /// Adds letter char to inpu playerInput string
    /// Called from interaction with letter tile uiObject
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="rectTransform"></param>
    void AddPlayerInput(string letter, RectTransform rectTransform)
    {
        playerInput += letter;
    }

    /// <summary>
    /// Removes letter char to from playerInput
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="rectTransform"></param>
    void RemovePlayerInput(string letter, RectTransform rectTransform)
    {
        string newString = playerInput.Replace(letter, string.Empty);
        playerInput = newString;
    }


    void ValidatePlayerInput()
    {
        OnLevelInputManagerValidatePlayerInput?.Invoke(playerInput);
    }

    public void ClearPlayerInput(string input)
    {
        playerInput = string.Empty;
    }

    public void OnCorrectInput()
    {
        OnLevelInputManagerCorrectInput?.Invoke();
    }



    private void OnDisable()
    {
        InputLetterTile.OnInputLetterTileSelected -= AddPlayerInput;
        InputLetterTile.OnInputLetterTileDeselected -= RemovePlayerInput;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputInvalid -= ClearPlayerInput;
    }
}
