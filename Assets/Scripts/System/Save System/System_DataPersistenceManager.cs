using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine. SceneManagement;

public class System_DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging / Development")]
    [SerializeField] private bool initializeDataIfNull = false; // For testing purposes, if no save data is found, it will create a new game.

    [Header("File Storage Config")]
    [SerializeField] private string dataFileName = "";

    public System_GameData gameData;
    private List<System_IDataPersistence> dataPersistenceObjects;
    private System_FileDataHandler dataHandler;

    public static System_DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            // This creates the data handler with a safe file path and your specified file name.
            this.dataHandler = new System_FileDataHandler(Application.persistentDataPath, dataFileName);
            LoadGame();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new System_GameData();
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (System_IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Started a new game. Last checkpoint ID is now: " + gameData.lastCheckpointID);
    }

    public void LoadGame()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        this.gameData = dataHandler.Load(); // When the save data doesn't exist, this will return null > and will create a new game.

        foreach (System_IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        Debug.Log("Game Loaded Data = " + "Health: " + gameData.currentPlayerHealth + ", Position: " + gameData.playerPosition);
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("No data to save. A new game must be started before data can be saved.");
            return;
        }
        // Find all data persistence objects in the current scene right before saving.
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        // Pass the current game data to other scripts  so they can update it
        foreach (System_IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        Debug.Log("Game Saved Data = " + "Health: " + gameData.currentPlayerHealth + ", Position: " + gameData.playerPosition);

        // Save all the data to a file using the Data Handler
        dataHandler.Save(gameData);
    }

    private List<System_IDataPersistence> FindAllDataPersistenceObjects()
    {
        // Find all the scripts in the scene that implements the IDataPersistence interface, but those scripts MUST be [MonoBehaviour]!!
        IEnumerable<System_IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<System_IDataPersistence>();
        return new List<System_IDataPersistence>(dataPersistenceObjects);
    }
    
    public bool HasGameData()
    {
        return gameData != null;
    }
}
