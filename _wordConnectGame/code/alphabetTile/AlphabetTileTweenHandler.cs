using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

public class AlphabetTileTweenHandler : MonoBehaviour
{
    public float duration = 1.0f;
    public float strengthPosition = 1.0f; 
    public float strengthRotation = 15.0f;
    public int vibrato = 10;
    public float randomness = 90.0f;

    public static Action OnTweenCollectionFXComplete;

    Vector3 startPos = Vector3.zero;

    private void Start()
    {
        startPos = transform.position;
    }


    public void TweenCollectionFX(Vector3 startPos, Vector3 endPos, float delay)
    {
        Vector3 midpoint = (startPos + endPos) / 2;
        midpoint.y = Mathf.Max(startPos.y, endPos.y) + 2.0f; // Raise the midpoint above the start and end positions
        Vector3[] pathPoints = new Vector3[] {startPos, midpoint, endPos };


        Sequence sequence = DOTween.Sequence();
        sequence
        .Append(transform.DOPath(pathPoints, .7f, PathType.CatmullRom))
        .Insert(0, transform.DOLocalRotate(new Vector3(0, 360, 0), .7f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear)).OnComplete(OnCollect)
        .SetDelay(delay)
        .OnComplete
        (
            () =>
            {
                OnTweenCollectionFXComplete?.Invoke();
                StopTweens();
            }
        );
    }

    void OnCollect()
    {
        if (gameObject.activeInHierarchy)
        {
            SceneRegistry.Instance.alphabetTileSpawner.alphabetTilePool.Release(GetComponent<AlphabetTile>());
        }
    }


    [Button]
    private void StopTweens()
    {
        transform.DOKill();
        Vector3 v = Vector3.zero;
        transform.eulerAngles = v;
    }
}