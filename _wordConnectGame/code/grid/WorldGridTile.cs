using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.ComponentModel;
using DG.Tweening;

public class WorldGridTile : MonoBehaviour
{
    public enum TileType
    {
        Default,
        Occupied,
        Merged,
        Required
    }


    public string tileId;
    public bool isOccupied;
    public bool playerInteractible;

    [Space]
    public TileType tileType;
    [Space]
    public Transform positionalTransform;
    public GameObject tileGeo;
    public AlphabetTile alphabetTile;

    [Space]
    public int fireProjectileCount = 1;
    public Transform[] projectiles;
    [Space]
    public GameObject defaultTile;
    public GameObject occupiedTile;
    public GameObject mergedTile;
    public GameObject lockedTile;

    [Space]
    public PlantObject plant;
    public PlantObject[] plantObjects;

    [Space]
    public List<WorldGridTile> neighbours = new List<WorldGridTile>();
    public static event System.Action<WorldGridTile> OnTileClicked;
    private void Start()
    {
        SetPlant();
    }

    public void SetTile(TileType tileType)
    {
        // Active tile based on the type
        this.tileType = tileType;
        

        switch (tileType)
        {
            case TileType.Default:
                defaultTile.SetActive(true);
                occupiedTile.SetActive(false);
                mergedTile.SetActive(false);
                lockedTile.SetActive(false);
                break;
            case TileType.Occupied:
                defaultTile.SetActive(false);
                occupiedTile.SetActive(true);
                mergedTile.SetActive(false);
                lockedTile.SetActive(false);
                break;
            case TileType.Merged:
                defaultTile.SetActive(false);
                occupiedTile.SetActive(false);
                mergedTile.SetActive(true);
                lockedTile.SetActive(false);
                break;
            case TileType.Required:
                defaultTile.SetActive(false);
                occupiedTile.SetActive(false);
                mergedTile.SetActive(false);
                lockedTile.SetActive(true);
                break;
        }
    }

    public void UpdatePlantLevel(WorldGridTile tile)
    {
        plant.Grow();
        tile.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 5, .5f).OnComplete(() => tile.transform.localScale = Vector3.one);
    }

    public void SetPlant()
    {
        foreach (var p in plantObjects)
        {
            p.gameObject.SetActive(false);
        }
        plant = plantObjects[Random.Range(0, plantObjects.Length)];
        plant.gameObject.SetActive(true);
    }

    private void OnMouseDown()
    {
        if (playerInteractible)
        {
            if (alphabetTile == null)
            {
                tileGeo.transform.DOPunchScale(Vector3.one * 0.2f, 1f, 5, .5f).OnComplete(() => tileGeo.transform.localScale = Vector3.one);
                OnTileClicked?.Invoke(this);
            }
        }
    }

    public void FireSeedProjectile()
    {
        for (int i = 0; i < fireProjectileCount; i++)
        {
            ParticleSystem s = null;
            if (projectiles[i] != null)
            {
                if (projectiles[i].GetComponent<ParticleSystem>())
                {
                    s = projectiles[i].GetComponent<ParticleSystem>();
                    s.Play();
                }

                // Capture targetTile for this specific projectile
                WorldGridTile currentTargetTile = GetPlantableNeigbourTile();

                if (currentTargetTile == null)
                {
                    Debug.LogWarning($"No plantable neighbour tile found for projectile {i}. Skipping this projectile.");
                    // Continue the loop to try for the next projectile if others exist
                    continue;
                }

                Vector3 startPos = positionalTransform.position;
                Vector3 targetPos = currentTargetTile.transform.position; // Use currentTargetTile
                Vector3 midPoint = (startPos + targetPos) / 2;
                midPoint.y += 2f; // Raise the midpoint above the start and end positions

                projectiles[i].transform.position = startPos;

                // Store s and currentTargetTile in local variables for the closure
                ParticleSystem projectileParticleSystem = s;
                WorldGridTile projectileTargetTile = currentTargetTile;

                projectiles[i].transform.DOPath(new Vector3[] { startPos, midPoint, targetPos }, 1f, PathType.CatmullRom)
                    .SetDelay(i * 0.25f) // Delay each projectile slightly
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (projectileParticleSystem != null)
                        {
                            projectileTargetTile.UpdatePlantLevel(projectileTargetTile); // Use the captured projectileTargetTile

                            projectileParticleSystem.Stop();
                        }
                        else
                        {
                            Debug.LogWarning("ParticleSystem not found on projectile.");
                        }
                    });
            }
        }
    }

    [Button]
    public void GetNeibouringTiles()
    {
        neighbours.Clear();
        List<string> ids = new List<string>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f); // Adjust radius as needed
        foreach (Collider collider in colliders)
        {
            WorldGridTile tile = collider.GetComponent<WorldGridTile>();
            if (tile != null && tile.tileId != tileId && !ids.Contains(tile.tileId))
            {
                neighbours.Add(tile);
                ids.Add(tile.tileId);
            }

        }
    }

    public WorldGridTile GetPlantableNeigbourTile()
    {
        WorldGridTile tile = null;
        List<WorldGridTile> _neighbours = new List<WorldGridTile>();

        foreach (var neighbor in neighbours)
        {
            if (neighbor != null && neighbor.tileType == TileType.Default && !neighbor.isOccupied)
            {
                _neighbours.Add(neighbor);
            }
        }

        tile = _neighbours.Count > 0 ? _neighbours[Random.Range(0, _neighbours.Count)] : null;

        return tile;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        foreach (var neighbor in neighbours)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
                Gizmos.DrawSphere(neighbor.transform.position, 0.2f); // Optional: mark neighbor position
            }
        }
    }

    public void SetInteractible(bool interactible)
    {
        playerInteractible = interactible;
        GetComponent<Collider>().enabled = interactible; ;
    }

    public void Clear()
    {
        isOccupied = false;
        alphabetTile = null;
        plant.Reset();
    }
}
