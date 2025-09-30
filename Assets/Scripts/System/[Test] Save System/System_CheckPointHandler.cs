using UnityEngine;

public class PlayerCheckpointHandler : MonoBehaviour, System_IDataPersistence
{

    [Header("Checkpoint Data")]
    public string lastCheckpointId;
    private Vector3 respawnPosition;

    [Header("Default Spawn")]
    public Transform defaultSpawnPoint; // Assign Checkpoint_00 in inspector

    public void LoadData(System_GameData data)
    {
    }

    public void SaveData(ref System_GameData data)
    {
    }

    public void SetCheckpoint(string checkpointId, Vector3 position)
    {
        lastCheckpointId = checkpointId;
        respawnPosition = position;
    }

    public Vector3 GetRespawnPosition()
    {
        return respawnPosition;
    }
}
