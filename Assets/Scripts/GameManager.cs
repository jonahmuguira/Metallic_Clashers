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
    private string currentScene = "Tony";

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

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnApplicationQuit()
    {
        playerData.staminaInformation.value = StaminaManager.self.value;
        playerData.staminaInformation.timeLastPlayed = DateTime.Now.ToString();
        Save();
    }

    [ContextMenu("End Combat")]
    public void OnCombatEnd()
    {

    }

    public void OnStageSelectionEnd()
    {
        NextScene(1);
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

    private void NextScene(int sceneIndex)
    {
        StartCoroutine(LoadScene(sceneIndex));
    }

    private void AddSceneListeners()
    {
        switch (currentScene)
        {
            case "Combat":
                CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
                break;

            case "Tony":
                StageSelectionManager.self.onStageSelectionEnd.AddListener(OnStageSelectionEnd);
                break;

            default:
                break;
        }
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        AddSceneListeners();
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncOperation.isDone) { yield return null; }
    }
}
