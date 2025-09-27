using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class System_GameData
{
    public int currentPlayerHealth;
    public Vector3 playerPosition;
    public System_SerializableDictionary<string, bool> isTimeDataCardCollected;

    // the values defined in this constructor will be the Default/New Game values
    // the game starts with these values if there is no save data to load
    public System_GameData()
    {
        currentPlayerHealth = 100;
        playerPosition = new Vector3(4.5f, 0, 5f); // [CHANGE LATER] Temporary spawn position system, will be replaced by a proper Spawn Point system.
        isTimeDataCardCollected = new System_SerializableDictionary<string, bool>();
    }
}
