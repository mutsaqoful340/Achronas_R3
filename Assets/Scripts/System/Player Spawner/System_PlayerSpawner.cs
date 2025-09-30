using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class System_PlayerSpawner : MonoBehaviour
{
    [Header("Player Prefab")]
    [Tooltip("The player character prefab to spawn.")]
    public GameObject playerPrefab;

    [Header("Default Spawn Point")]
    [Tooltip("The Transform where the player will spawn if no save data or matching checkpoint is found.")]
    public Transform defaultSpawnPoint;

    private void OnEnable()
    {
        // Subscribe to the event that fires when the active scene changes.
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks.
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    /// <summary>
    /// This method is called by the SceneManager when a new scene is set to be the active one.
    /// </summary>
    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        // We only want to spawn the player if the NEW active scene is the one
        // that this spawner component is in.
        if (newScene == this.gameObject.scene)
        {
            SpawnPlayer();
            // After spawning, we can unsubscribe so this doesn't accidentally run again.
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || defaultSpawnPoint == null)
        {
            Debug.LogError("Player Prefab and/or Default Spawn Point are not assigned in the Player Spawner!");
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetSpawnPosition()
    {
        // Start with the default position as a fallback
        Vector3 position = defaultSpawnPoint.position;

        // Check if we have any saved data to begin with
        if (System_DataPersistenceManager.instance == null || System_DataPersistenceManager.instance.gameData == null)
        {
            Debug.Log("No game data found. Spawning at default location.");
            return position;
        }

        // Get the ID of the last checkpoint from our saved game data
        string lastCheckpointId = System_DataPersistenceManager.instance.gameData.lastCheckpointID;

        // If the ID is null or empty, it means we haven't hit a checkpoint yet
        if (string.IsNullOrEmpty(lastCheckpointId))
        {
            Debug.Log("No checkpoint ID saved. Spawning at default location.");
            return position;
        }

        // Find all checkpoint objects currently in the scene
        System_Checkpoint[] checkpoints = FindObjectsOfType<System_Checkpoint>();
        // Find the specific checkpoint that matches our saved ID
        System_Checkpoint lastCheckpoint = checkpoints.FirstOrDefault(c => c.checkpointID == lastCheckpointId);

        if (lastCheckpoint != null)
        {
            // If we found the matching checkpoint, use its position!
            position = lastCheckpoint.transform.position;
            Debug.Log("Found matching checkpoint '" + lastCheckpointId + "'. Spawning player there.");
        }
        else
        {
            // If the checkpoint from our save file doesn't exist in this scene
            Debug.LogWarning("Saved checkpoint ID '" + lastCheckpointId + "' not found in this scene. Spawning at default location.");
        }

        return position;
    }
}