using UnityEngine;

public class System_Checkpoint : MonoBehaviour, System_IDataPersistence
{
    [Header("Checkpoint ID")]
    public string checkpointID; // Unique ID like "Checkpoint_01_Forest"
    

    [Header("Checkpoint Icon")]
    public GameObject checkpointIcon; // Optional visual indicator

    [SerializeField] private bool hasBeenActivated = false; // Prevents it from triggering multiple times

    [SerializeField]private Player_InputHandle playerInputHandle;

    private bool playerIsInsideTrigger = false;

    [ContextMenu("Generate GUID for Checkpoint")]
    private void GenerateGuid()
    {
        checkpointID = System.Guid.NewGuid().ToString();
    }

    private void Update()
    {
        // Every frame, check if the player is inside AND has pressed the button.
        if (playerIsInsideTrigger && !hasBeenActivated && playerInputHandle != null && playerInputHandle.InteractPressed)
        {
            ActivateCheckpoint();
        }
    }

    // Delete your old OnTriggerEnter method and replace it with this.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInsideTrigger = true;
            playerInputHandle = other.GetComponent<Player_InputHandle>();
            // You could show a UI prompt here! e.g., promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInsideTrigger = false;
            playerInputHandle = null;
            // You would hide the UI prompt here! e.g., promptUI.SetActive(false);
        }
    }

    private void ActivateCheckpoint()
    {
        // Tell the data persistence manager to update the game data with THIS checkpoint's info
        if (System_DataPersistenceManager.instance != null)
        {
            System_DataPersistenceManager.instance.gameData.lastCheckpointID = this.checkpointID;
            System_DataPersistenceManager.instance.gameData.lastCheckpointPosition = this.transform.position;
            
            // Optional: You could add a visual/sound effect here
            Debug.Log("Checkpoint Activated: " + checkpointID);

            // You could set hasBeenActivated to true if you only want it to fire once
            hasBeenActivated = true; 
        }
    }

    public void LoadData(System_GameData data)
    {
        // Check if this checkpoint was the last one saved
        if (data.lastCheckpointID == this.checkpointID)
        {
            hasBeenActivated = true;
            // You could change the checkpoint's color or light to show it's active
        }
    }

    public void SaveData(ref System_GameData data)
    {
        // This script's purpose is to WRITE data via OnTriggerEnter, not during the save loop.
        // So, this method can remain empty.
    }
}