using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

using Board;

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

    private void Awake()
    {
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
    }

    private void OnCombatEnd()
    {
        
    }

    [ContextMenu("Save Player")]
    private void Save()
    {
        //Saving PlayerData
        var playerPath = Environment.CurrentDirectory + "/PlayerData.xml";
        var playerStream = File.Create(playerPath);

        var serializer = new XmlSerializer(typeof(PlayerData));
        serializer.Serialize(playerStream, playerData);
        playerStream.Close();
    }

    [ContextMenu("Load Player")]
    private void Load()
    {
        var reader = new XmlSerializer(typeof(PlayerData));
        var file = new StreamReader(Environment.CurrentDirectory + "/PlayerData.xml");

        playerData = (PlayerData) reader.Deserialize(file);
        file.Close();
    }
}
