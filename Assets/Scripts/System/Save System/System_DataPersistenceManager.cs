using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class System_DataPersistenceManager : MonoBehaviour
{
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
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        this.dataHandler = new System_FileDataHandler(Application.persistentDataPath, dataFileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects(); // Find all the scripts in the scene that implements the IDataPersistence interface
        LoadGame(); // [CHANGE LATER] Attempt to load game data when the PersistenceManager loaded, which is not good.
    }

    public void NewGame()
    {
        gameData = new System_GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load(); // When the save data doesn't exist, this will return null > and will create a new game.

        // Load any saved data from a file using the Data Handler
        // if no data can be loaded, initialize a new game (or disable the Continue Game button)

        // [CHANGE LATER] This is temporary, the Continue Button will be disabled if no save data is found.
        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        else
        {
            Debug.Log("Data loaded.");
        }

        // Push the loaded data to all other scripts that need it
        foreach (System_IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        Debug.Log("Game Loaded Data = " + "Health: " + gameData.currentPlayerHealth + ", Position: " + gameData.playerPosition);
    }

    public void SaveGame()
    {
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
}
