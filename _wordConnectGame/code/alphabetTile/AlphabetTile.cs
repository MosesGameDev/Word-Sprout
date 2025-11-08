using DG.Tweening;
using NUnit.Framework.Internal;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlphabetTileTweenHandler))]
public class AlphabetTile : MonoBehaviour
{
    public enum TileType
    {
        Alphabet,
        Merge
    }

    public string letter;
    public TileType tileType;
    [SerializeField] private MeshFilter meshFilter;
    [Space]
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject mergedTile;

    [Space]
    [SerializeField] private AlphabetLetterGameObject[] alphabetLetterGameObjects;

    [Space]
    [SerializeField] private Color mergeColor;
    [Space]
    public WorldGridTile gridTile;
    AlphabetTileTweenHandler tweenHandler;
    Color defaultColor;

    private void Start()
    {
        tweenHandler = GetComponent<AlphabetTileTweenHandler>();
    }


    public void SetLetterMesh(string _letter)
    {
        letter = _letter;
        for (int i = 0; i < alphabetLetterGameObjects.Length; i++)
        {
            if (alphabetLetterGameObjects[i]._letter == _letter)
            {
                meshFilter.mesh = alphabetLetterGameObjects[i]._mesh;
            }
        }
    }


    public void MoveToCollectionPoint(Vector3 position, float delay)
    {
        tweenHandler.TweenCollectionFX(transform.position, position, delay);
    }

    [Button]
    public void PlayMergeFx()
    {
        Sequence fxSequence = DOTween.Sequence();

        fxSequence
        .Append(transform.DOPunchScale(Vector3.one * 0.5f, 0.5f))
        .Insert(0, transform.DOLocalRotate(new Vector3(0, 360, 0), fxSequence.Duration(), RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear))
        .OnComplete(() => { SetTileType(TileType.Merge); });
    }

    public void SetTileType(TileType type)
    {
        tileType = type;
        switch(type)
        {
            case TileType.Alphabet:
                tile.SetActive(true);
                mergedTile.SetActive(false);
                break;
            case TileType.Merge:
                tile.SetActive(false);
                mergedTile.SetActive(true);
                break;
        }
    }

    public void Reset()
    {
        SetTileType(TileType.Alphabet);
    }
}


[System.Serializable]
public class AlphabetLetterGameObject
{
    public string _letter;
    public Mesh _mesh;
}