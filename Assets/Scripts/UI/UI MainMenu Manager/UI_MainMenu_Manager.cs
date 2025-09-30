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
    [Tooltip("The name of the gameplay scene to load.")]
    [SerializeField] private string sceneToLoadName = "GameplayScene"; // Example name
    [Tooltip("The name of the main menu scene to unload.")]
    [SerializeField] private string sceneToUnloadName = "MainMenu"; // Example name

    void Start()
    {
        // This logic is still correct and doesn't need to be changed.
        if (System_DataPersistenceManager.instance != null && !System_DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
            Debug.Log("No Save Data Found - Continue Button Disabled");
        }
    }

    public void OnClick_NewGame()
    {
        DisableMenuButtons();

        // --- CHANGE ---
        // Call the new, cleaner method from the loading screen script.
        System_UniversalLoadingScreen.instance.LoadNewGame(sceneToLoadName, sceneToUnloadName);
    }

    public void OnClick_ContinueGame()
    {
        DisableMenuButtons();

        // --- CHANGE ---
        // Call the new, cleaner method for loading a saved game.
        System_UniversalLoadingScreen.instance.LoadSavedGame(sceneToLoadName, sceneToUnloadName);
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