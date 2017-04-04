using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

using Combat;

using Library;

using StageSelection;

using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState
    {
        Title,
        StateSelection,
        Combat,
        Credits,
    }

    [SerializeField]
    private UnityEvent m_OnSceneLoaded = new UnityEvent();

    private const string savePath = "/PlayerData.xml";
    [SerializeField]
    private List<GameObject> m_EnemyPrefabList = new List<GameObject>();

    public GameState gameState;

    public PlayerData playerData;

    public List<int> enemyIndexes = new List<int>();

    public List<GameObject> enemyPrefabList { get { return m_EnemyPrefabList; } }
    public UnityEvent onSceneLoaded { get { return m_OnSceneLoaded; } }

    protected override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
        if (File.Exists(Environment.CurrentDirectory + savePath))
            LoadPlayer();
        else
        {
            playerData = new PlayerData(200, 10, 10);
            playerData.staminaInformation = new StaminaInformation
            {
                value = 0,
                timeLastPlayed = DateTime.Now.ToString()
            };
            SavePlayer();
        }

        gameState = (GameState)SceneManager.GetActiveScene().buildIndex;
        AddSceneListeners();
        //onSceneLoaded.AddListener(AddSceneListeners);
    }

    //private void OnApplicationQuit()
    //{
    //    playerData.staminaInformation.maxValue = StaminaManager.self.maxValue;
    //    playerData.staminaInformation.value = StaminaManager.self.value;
    //    playerData.staminaInformation.timeLastPlayed = DateTime.Now.ToString();
    //    SavePlayer();
    //}

    private void OnCombatEnd()
    {
        LoadScene((int)GameState.StateSelection);
    }

    private void OnStageSelectionEnd()
    {
        LoadScene((int)GameState.Combat);
    }

    private void AddSceneListeners()
    {
        switch (gameState)
        {
            case GameState.Credits:
                var button = FindObjectOfType<Button>();
                button.onClick.AddListener(() => { LoadScene(0); });
                break;

            case GameState.Combat:     // Combat
                playerData.health.modifier = 0;
                playerData.defense.modifier = 0;
                CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
                CombatManager.self.onPlayerTurn.AddListener(AudioManager.self.PlayDragSound);
                gameState = GameState.Combat;

                // Toggle Music Button
                GameObject.Find("Menu Button").transform.FindChild("Icon Layout Group")
                    .FindChild("Music Button").gameObject.GetComponent<Button>
                    ().onClick.AddListener(AudioManager.self.MuteMusicToggle);

                // Toggle SoundEffect Button
                GameObject.Find("Menu Button").transform.FindChild("Icon Layout Group")
                    .FindChild("Sound Effects Button").gameObject.GetComponent<Button>
                    ().onClick.AddListener(AudioManager.self.MuteSoundsToggle);
                break;

            case GameState.StateSelection:     // Stage Selection
                StageSelectionManager.self.onStageSelectionEnd.AddListener
                    (OnStageSelectionEnd);

                GameObject.Find("Title").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(0); });
                break;

            case GameState.Title:
                GameObject.Find("Play").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(1); });

                GameObject.Find("Credits").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(3); });
                break;
        }
        AudioManager.self.ChangeMusic((int)gameState);
    }

    [ContextMenu("Save Player")]
    private void SavePlayer()
    {
        //Saving PlayerData
        var playerPath = Environment.CurrentDirectory + savePath;
        var playerStream = File.Create(playerPath);

        var serializer = new XmlSerializer(typeof(PlayerData));
        serializer.Serialize(playerStream, playerData);
        playerStream.Close();

        playerData.itemManager.SaveItems();
    }

    [ContextMenu("Load Player")]
    private void LoadPlayer()
    {
        var reader = new XmlSerializer(typeof(PlayerData));
        var file = new StreamReader(Environment.CurrentDirectory + savePath);

        playerData = (PlayerData)reader.Deserialize(file);
        file.Close();

        playerData.itemManager.LoadItems();
    }

    public void LoadScene(int sceneIndex)
    {
        if ((int)gameState != sceneIndex)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex));
        }

    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncOperation.isDone) { yield return null; }

        gameState = (GameState)sceneIndex;

        AddSceneListeners();
        onSceneLoaded.Invoke();
    }

}