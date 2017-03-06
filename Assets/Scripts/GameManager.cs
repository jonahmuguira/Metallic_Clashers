using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

using Library;

using StageSelection;

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

    private int currentScene = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        if (File.Exists(Environment.CurrentDirectory + savePath))
            Load();
        else
        {
            playerData.staminaInformation = new StaminaInformation
            {
                value = 0,
                timeLastPlayed = DateTime.Now.ToString()
            };
            Save();
        }
        AddSceneListeners();
    }

    private void OnApplicationQuit()
    {
        playerData.staminaInformation.value = StaminaManager.self.value;
        playerData.staminaInformation.timeLastPlayed = DateTime.Now.ToString();
        Save();
    }

    public void OnCombatEnd()
    {
        NextScene(1);
    }

    public void OnStageSelectionEnd()
    {
        NextScene(2);
    }

    private void AddSceneListeners()
    {
        switch (currentScene)
        {
            case 2:
                CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
                break;

            case 1:
                StageSelectionManager.self.onStageSelectionEnd.AddListener(OnStageSelectionEnd);
                break;

            default:
                break;
        }
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

        playerData = (PlayerData)reader.Deserialize(file);
        file.Close();
    }

    public void NextScene(int sceneIndex)
    {
        StartCoroutine(LoadScene(sceneIndex));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        currentScene = sceneIndex;

        while (!asyncOperation.isDone) { yield return null; }

        currentScene = sceneIndex;
        AddSceneListeners();
    }
}
