using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface System_IDataPersistence
{
    void LoadData(System_GameData data); // Allow all the implementing scripts which only cares to ONLY READ the data.
    void SaveData(ref System_GameData data); // Allow all the implementing scripts to MODIFY the data. 
}
