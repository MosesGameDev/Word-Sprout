using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class InputLineRenderer : MonoBehaviour
{
    UILineRenderer lineRenderer;
    [SerializeField] private UILineRenderer connectorLineRenderer;
    [SerializeField] private List<RectTransform> selectedTiles = new List<RectTransform>();

    private void OnEnable()
    {
        InputLetterTile.OnInputLetterTileSelected += AddPoint;
        InputLetterTile.OnInputLetterTileDeselected += RemovePoint;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputInvalid += ClearLineRenderer;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputValid += ClearLineRenderer;

        PlayerInputDisplay.OnPlayerInputDisplayValidInput += ClearLineRenderer;
    }

    private void Start()
    {
        lineRenderer = GetComponent<UILineRenderer>();
    }


    void AddPoint(string letter, RectTransform tileRectTransform)
    {
        if (selectedTiles.Contains(tileRectTransform))
        {
            return;
        }

        selectedTiles.Add(tileRectTransform);
        UpdateLineRender();
    }

    void RemovePoint(string letter, RectTransform tileRectTransform)
    {
        if (!selectedTiles.Contains(tileRectTransform))
        {
            return;
        }

        selectedTiles.Remove(tileRectTransform);
        UpdateLineRender();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (selectedTiles.Count > 0)
            {
                connectorLineRenderer.SetAllDirty();

                int j = selectedTiles.Count - 1;
                connectorLineRenderer.Points[0] = selectedTiles[j].anchoredPosition;
                connectorLineRenderer.Points[1] = SceneRegistry.Instance.levelInputManager.GetPointerRect().anchoredPosition;

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            connectorLineRenderer.SetAllDirty();
            connectorLineRenderer.Points[0] = Vector2.zero;
            connectorLineRenderer.Points[1] = Vector2.zero;

        }


    }


    void UpdateLineRender()
    {
        lineRenderer.Points = null;
        List<Vector2> rectTransformPositions = new List<Vector2>();

        for (int i = 0; i < selectedTiles.Count; i++)
        {
            if (!rectTransformPositions.Contains(selectedTiles[i].anchoredPosition))
            {
                rectTransformPositions.Add(selectedTiles[i].anchoredPosition);
            }
        }

        lineRenderer.Points = rectTransformPositions.ToArray();

    }

    void ClearLineRenderer(string input)
    {
        lineRenderer.Points = null;
    }

    void ClearLineRenderer()
    {
        lineRenderer.Points = null;
        selectedTiles.Clear();
    }



    private void OnDisable()
    {
        InputLetterTile.OnInputLetterTileSelected -= AddPoint;
        InputLetterTile.OnInputLetterTileDeselected -= RemovePoint;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputInvalid -= ClearLineRenderer;
        LevelWordsDictionary.OnLevelWordsDictionaryPlayerInputValid -= ClearLineRenderer;
        PlayerInputDisplay.OnPlayerInputDisplayValidInput -= ClearLineRenderer;

    }

}
