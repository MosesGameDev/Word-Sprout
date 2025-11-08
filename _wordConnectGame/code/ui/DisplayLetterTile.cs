using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using UnityEngine.ProBuilder.Shapes;

public class DisplayLetterTile : MonoBehaviour
{
    public string value;
    public string targetTileId;
    [Space]
    public TextMeshProUGUI textMeshProUGUI;

    [Sirenix.OdinInspector.ReadOnly]
    public GridTileBase targetGridTile;
    public WorldGridTile targetWorldGridTile;

    RectTransform _rectTransform;

    // Keep track of active coroutines
    private Coroutine destroyTileCoroutine;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string text)
    {
        value = text;
        textMeshProUGUI.SetText(value);
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

    public void DestroyTile(Color col)
    {

        Vector3 punch = new Vector3(0, 0, -15);
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(_rectTransform.DOScale(Vector3.zero, .3f).SetEase(Ease.InBack))
            .Insert(0, _rectTransform.DOPunchRotation(punch, sequence.Duration(), 8, 1));

        // Replace Invoke with coroutine
        if (destroyTileCoroutine != null)
        {
            StopCoroutine(destroyTileCoroutine);
        }
        destroyTileCoroutine = StartCoroutine(DestroyTileDelayedCoroutine(1f));
    }

    // Coroutine to replace the Invoke("OnDestroyTile", 1f) call
    private IEnumerator DestroyTileDelayedCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnDestroyTile();
    }

    private void OnDestroyTile()
    {
        Destroy(gameObject);
    }

    public void MoveAlphabetToTargetTile(WorldGridTile gridtile, float delay)
    {
        StartCoroutine(AlphabetTileMoveToTargetTileCoroutine(gridtile, delay));
    }


    private IEnumerator AlphabetTileMoveToTargetTileCoroutine(WorldGridTile gridtile, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        Vector3 spawnPosition = SceneRegistry.Instance.alphabetTileSpawner.GetSpawnPosition().position;
        Vector3 worldPos = gridtile.positionalTransform.position;

        AlphabetTile alphabetTile = SceneRegistry.Instance.alphabetTileSpawner.alphabetTilePool.Get();
        alphabetTile.transform.localScale = Vector3.one * 0.1f;

        SceneRegistry.Instance.alphabetTileSpawner.spawnedAlpabetTiles.Add(alphabetTile);

        alphabetTile.transform.eulerAngles = Vector3.zero;
        alphabetTile.SetLetterMesh(value);
        alphabetTile.transform.eulerAngles = new Vector3(75, 0, 0);
        alphabetTile.transform.position = spawnPosition;

        Sequence sequence = DOTween.Sequence();

        alphabetTile.transform.DOScale(Vector3.one, .5f);
        sequence
        .Append(alphabetTile.transform.DOMove(worldPos, .8f).SetEase(Ease.OutBounce))
        .Insert(0, alphabetTile.transform.DOLocalRotate(new Vector3(0, 360, 0), sequence.Duration(), RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear))
        .OnComplete
        (
            () =>
            {
                WorldGridTile.TileType tileType = WorldGridTile.TileType.Default;

                //Am destroying duplicate alphabet letter tiles if the tiles has alphabetTile
                if (gridtile.alphabetTile != null)
                {
                    SceneRegistry.Instance.alphabetTileSpawner.alphabetTilePool.Release(alphabetTile);
                    gridtile.alphabetTile.PlayMergeFx();
                    tileType = WorldGridTile.TileType.Merged;
                }
                else
                {
                    tileType = WorldGridTile.TileType.Occupied;
                }

                gridtile.plant.worldGridTile = gridtile;

                gridtile.FireSeedProjectile();

                gridtile.SetTile(tileType);
                gridtile.alphabetTile = alphabetTile;
                alphabetTile.transform.DOPunchScale(Vector3.one * 0.25f, .3f);
                OnMoveToTargetTile();
            }
        );
    }

    private void OnMoveToTargetTile()
    {
        //SceneRegistry.Instance.tileGridManager.GetCurrentGrid().SetTileNull(targetGridTile.tilePosition);
        Destroy(gameObject);
    }

    // Make sure to stop any running coroutines when the object is disabled or destroyed
    private void OnDisable()
    {
        if (destroyTileCoroutine != null)
        {
            StopCoroutine(destroyTileCoroutine);
            destroyTileCoroutine = null;
        }
    }
}