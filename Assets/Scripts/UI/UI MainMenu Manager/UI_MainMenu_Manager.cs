#region Summary
// This script is used to manage the main menu UI interactions such as starting a new game, loading a game, and quitting the application.
#endregion

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu_Manager : MonoBehaviour
{
    [Header("Main Menu UI Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] public Button continueGameButton;
    [SerializeField] private Button quitGameButton;

    [Header("Scene Names")]
    [SerializeField] private string sceneToLoadName;
    [SerializeField] private string sceneToUnloadName;

    void Start()
    {
        if (!System_DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false; // Disable Continue Button if no save data is found.
        }
    }
    public void OnClick_NewGame()
    {
        DisableMenuButtons();
        // Create a New Game - which will initialize the game data to default values
        //System_DataPersistenceManager.instance.NewGame();
        System_UniversalLoadingScreen.instance.sceneToLoadName = this.sceneToLoadName;
        System_UniversalLoadingScreen.instance.sceneToUnloadName = this.sceneToUnloadName;
        System_UniversalLoadingScreen.instance.SetIsNewGame(true);


        //Debug.Log("New Game Clicked");
    }

    public void OnClick_ContinueGame()
    {
        DisableMenuButtons();
        System_UniversalLoadingScreen.instance.sceneToLoadName = this.sceneToLoadName;
        System_UniversalLoadingScreen.instance.sceneToUnloadName = this.sceneToUnloadName;
        System_UniversalLoadingScreen.instance.SetIsLoadGame(true);
        Debug.Log("Continue Game Clicked");
    }

    public void OnClick_QuitGame()
    {
        DisableMenuButtons();
        Debug.Log("Quit Game Clicked");
        Application.Quit();
    }

    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
        quitGameButton.interactable = false;
    }
}
