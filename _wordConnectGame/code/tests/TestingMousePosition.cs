using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;


public class TestingMousePosition : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public Canvas parentCanvas; // Assign your canvas here

    private void Update()
    {
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.worldCamera,
            out movePos
        );

        transform.position = parentCanvas.transform.TransformPoint(movePos);

    }
}
