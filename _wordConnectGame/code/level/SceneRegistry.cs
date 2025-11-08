using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRegistry : MonoBehaviour
{
    public static SceneRegistry Instance;

    public LetterTileGridManager tileGridManager;
    public WorldGrid worldGrid;
    public LevelInputManager levelInputManager;
    public PlayerInputHandler playerInputHandler;
    public AlphabetTileSpawner alphabetTileSpawner;
    public HarvestOrdersHandler harvestOrdersHandler;
    public PowerUpHandler powerUpHandler;

    private void Awake()
    {
        Instance = this;
    }

}
