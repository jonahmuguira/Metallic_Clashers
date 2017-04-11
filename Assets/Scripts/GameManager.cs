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

using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState
    {
        Title,
        StateSelection,
        Preparation,
        Combat,
        Credits,
    }

    [SerializeField]
    private UnityEvent m_OnSceneLoaded = new UnityEvent();

    private string m_PlayerSavePath;
    private string m_InventorySavePath;
    [SerializeField]
    private List<GameObject> m_EnemyPrefabList = new List<GameObject>();

    private int m_RandomSeed;

    public GameState gameState;

    public PlayerData playerData = new PlayerData();

    public List<int> enemyIndexes = new List<int>();

    public List<GameObject> enemyPrefabList { get { return m_EnemyPrefabList; } }
    public UnityEvent onSceneLoaded { get { return m_OnSceneLoaded; } }

    public int randomSeed { get { return m_RandomSeed; } }

    protected override void OnAwake()
    {
        m_RandomSeed = (int)DateTime.Now.Ticks;
        Random.InitState(m_RandomSeed);

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
                value = 0,
                maxValue = 100,
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

            case GameState.Preparation: //this is where you select items/gem types before combat
                GameObject.Find("Accept").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(1); });
                break;

            case GameState.StateSelection:     // Stage Selection
                StageSelectionManager.self.onStageSelectionEnd.AddListener
                    (OnStageSelectionEnd);

                GameObject.Find("Title").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(0); });

                GameObject.Find("Setup").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(2); });
                break;

            case GameState.Title:
                GameObject.Find("Play").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(1); });

                GameObject.Find("Credits").gameObject.GetComponent<Button>().onClick.AddListener(() => { LoadScene(4); });
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

        playerData.itemManager.SaveItems(m_InventorySavePath);
    }

    [ContextMenu("Load Player")]
    private void LoadPlayer()
    {
        var reader = new XmlSerializer(typeof(PlayerData));
        var file = new StreamReader(m_PlayerSavePath);

        playerData = (PlayerData)reader.Deserialize(file);
        file.Close();

        playerData.itemManager.LoadItems(m_InventorySavePath);
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