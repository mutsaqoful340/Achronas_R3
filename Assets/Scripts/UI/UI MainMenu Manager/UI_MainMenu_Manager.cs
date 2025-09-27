using UnityEngine;

public class UI_MainMenu_Manager : MonoBehaviour
{
    public void OnClick_NewGame()
    {
        System_DataPersistenceManager.instance.NewGame();
        Debug.Log("New Game Clicked");
    }

    public void OnClick_LoadGame()
    {
        System_DataPersistenceManager.instance.LoadGame();
        Debug.Log("Load Game Clicked");
    }

    public void OnClick_QuitGame()
    {
        Debug.Log("Quit Game Clicked");
        Application.Quit();
    }
}
