using UnityEngine;

public class TestUI_Gameplay : MonoBehaviour
{
    public void OnClick_SaveGane()
    {
        System_DataPersistenceManager.instance.SaveGame();
        Debug.Log("Save Game Clicked");
    }

    public void OnClick_LoadGame()
    {
        System_DataPersistenceManager.instance.LoadGame();
        Debug.Log("Load Game Clicked");
    }
}
