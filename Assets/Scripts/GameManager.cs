using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

using Library;

using UnityEngine.SceneManagement;

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
    private string currentScene;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        if(File.Exists(Environment.CurrentDirectory + savePath))
            Load();
        else
        {
            Save();
        }
    }

    [ContextMenu("On Combat End")]
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

    private void NextScene(string nextScene)
    {
        SceneManager.LoadSceneAsync(nextScene);
        currentScene = nextScene;
    }
}
