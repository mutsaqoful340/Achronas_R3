#region Summary
// This script is used for testing the gameplay mechanics functionality such as saving and loading the game through UI interaction.
#endregion

using UnityEngine;

public class TestUI_Gameplay : MonoBehaviour
{
    [Header("Scene Names")]
    public string sceneToLoadName;
    public string sceneToUnloadName;

    public void OnClick_SaveGame()
    {
        System_DataPersistenceManager.instance.SaveGame();
        Debug.Log("Save Game Clicked");
    }

    public void OnClick_LoadGame()
    {
        System_UniversalLoadingScreen.instance.TransitionToScene(sceneToLoadName, sceneToUnloadName);
        Debug.Log("Load Game Clicked");
    }
    
    public void OnClick_ReturnToMainMenu()
    {
        System_UniversalLoadingScreen.instance.TransitionToScene(sceneToLoadName, sceneToUnloadName);
        Debug.Log("Return to Main Menu Clicked");
    }
}
