using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Combat;
using Items;
using Library;
using StageSelection;

using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState
    {
        Title,
        StageSelection,
        Combat,
        Credits,
    }

    [SerializeField]
    private UnityEvent m_OnSceneLoaded = new UnityEvent();

    private string m_PlayerSavePath;
    private string m_InventorySavePath;
    [SerializeField]
    private List<GameObject> m_EnemyPrefabList = new List<GameObject>();
    
    public GameState gameState;

    public Node currentNode;

    public PlayerData playerData = new PlayerData();

    public List<int> enemyIndexes = new List<int>();

    public List<GameObject> enemyPrefabList { get { return m_EnemyPrefabList; } }
    public UnityEvent onSceneLoaded { get { return m_OnSceneLoaded; } }
    
    protected override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
        m_InventorySavePath = Application.persistentDataPath + "/Inventory.xml";
        m_PlayerSavePath = Application.persistentDataPath + "/PlayerData.xml";

        if (File.Exists(m_PlayerSavePath))
            LoadPlayer();
        else
        {
            playerData = new PlayerData(200, 10, 10);
            playerData.staminaInformation = new StaminaInformation
            {
                value = 10,
                maxValue = 100,
                timeLastPlayed = DateTime.Now.ToString()
            };
            StaminaManager.self.SetStam();
            SavePlayer();
        }

        playerData.playerLevelSystem.playerLevelInfo.level =
            (playerData.playerLevelSystem.playerLevelInfo.level <= 0)
                ? 1
                : playerData.playerLevelSystem.playerLevelInfo.level;

        playerData.playerLevelSystem.playerLevelInfo.experienceRequired =
        (playerData.playerLevelSystem.playerLevelInfo.experienceRequired < 200)
            ? 200
            : playerData.playerLevelSystem.playerLevelInfo.experienceRequired;

        //instant = 25 health 
        //turn attack up = 7 turns 100 buff, turn defense up = 7 turns 100 buff
        //time attack up = 15 secs 100 buff, time defense up = 15 secs 100 buff
        for (int i = 0; i < 2; i++)
        {
            playerData.itemManager.AddInventoryItem(new InstantItem(0.1f));
        }

        gameState = (GameState)SceneManager.GetActiveScene().buildIndex;
        AddSceneListeners();
        //onSceneLoaded.AddListener(AddSceneListeners);
    }

    private void OnApplicationQuit()
    {
        SavePlayer();
    }

    private void OnCombatEnd()
    {
        LoadScene((int)GameState.StageSelection);
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

            case GameState.StageSelection:     // Stage Selection
                StageSelectionManager.self.onStageSelectionEnd.AddListener
                    (OnStageSelectionEnd);

                GameObject.Find("Title Button").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(0); });
                break;

            case GameState.Title:
                GameObject.Find("Play").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(1); });

                GameObject.Find("Credits").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(3); });
                break;
        }
        AudioManager.self.ChangeMusic((int)gameState);
    }

    [ContextMenu("Save Player")]
    public void SavePlayer()
    {
        playerData.staminaInformation = new StaminaInformation
        {
            value = StaminaManager.self.value,
            maxValue = StaminaManager.self.maxValue,
            timeLastPlayed = DateTime.Now.ToString(),
        };

        //Saving PlayerData
        var playerPath = m_PlayerSavePath;
        var playerStream = File.Create(playerPath);

        var serializer = new XmlSerializer(typeof(PlayerData));
        serializer.Serialize(playerStream, playerData);
        playerStream.Close();

        //playerData.itemManager.SaveItems(m_InventorySavePath);
    }

    [ContextMenu("Load Player")]
    private void LoadPlayer()
    {
        var reader = new XmlSerializer(typeof(PlayerData));
        var file = new StreamReader(m_PlayerSavePath);

        playerData = (PlayerData)reader.Deserialize(file);
        file.Close();

        //playerData.itemManager.LoadItems(m_InventorySavePath);
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