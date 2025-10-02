using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class System_GameData
{
    // Player Health
    public int currentPlayerHealth;

    // Player Position
    public Vector3 playerPosition;

    // Checkpoiunt
    public string lastCheckpointID;
    public Vector3 lastCheckpointPosition;

    // Time Data Cards 
    public System_SerializableDictionary<string, bool> isTimeDataCardCollected;
    //public System_SerializableDictionary<string, bool> matchCheckpointID; // <== this one

    // the values defined in this constructor will be the Default/New Game values
    // the game starts with these values if there is no save data to load
    public System_GameData()
    {
        currentPlayerHealth = 100;
        playerPosition = new Vector3(4.5f, 0, 5f); // Currently not used by the Player Spawner, but might be useful later.
        isTimeDataCardCollected = new System_SerializableDictionary<string, bool>();
        lastCheckpointID = "Checkpoint_00"; // Default starting checkpoint ID
    }
}
