using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

using Board;

using Library;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState
    {
        Combat,
        MainMenu,
        StageSelection,
        StagePreparation,
    }

    private const string savePath = "/PlayerData.xml";

    public GameState gameState;

    public PlayerData playerData;

    protected override void Awake()
    {
        base.Awake();
        CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
    }

    private void OnCombatEnd()
    {
        
    }

    [ContextMenu("Save Player")]
    private void Save()
    {
        //Saving PlayerData
        var playerPath = Environment.CurrentDirectory + savePath;
        var playerStream = File.Create(playerPath);

        var serializer = new XmlSerializer(typeof(PlayerData));
        serializer.Serialize(playerStream, playerData);
        playerStream.Close();
    }

    [ContextMenu("Load Player")]
    private void Load()
    {
        var reader = new XmlSerializer(typeof(PlayerData));
        var file = new StreamReader(Environment.CurrentDirectory + savePath);

        playerData = (PlayerData) reader.Deserialize(file);
        file.Close();
    }
}
