using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Combat,
        MainMenu,
        StageSelection,
        StagePreparation,
    }

    public GameState gameState;

    public PlayerData playerData;
    private string savePath;

    private void Awake()
    {

    }

    private void Save()
    {
        ////TODO: This is where the player's data will be Saved out
    }

    private void Load()
    {
        ////TODO: This is where the player's data will be Loaded in
    }
}
