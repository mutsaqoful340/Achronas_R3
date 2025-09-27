#region Script_Summary
// === Summary ===
// This script boots the game, loads PersistentManager and MainMenu.
// The active scene should always be MainMenu. 
// PersistentManager should never be set active since it only holds managers.
#endregion

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class System_BootScene : MonoBehaviour
{
    [Header("Boot Time Delay")]
    [SerializeField] private float bootTimeDelay = 3f;
    [SerializeField] private string persistentManagerScene;
    [SerializeField] private string mainMenuScene;

    private void Start()
    {
        StartCoroutine(BootRoutine());
    }

    private IEnumerator BootRoutine()
    {
        // Wait before loading
        yield return new WaitForSeconds(bootTimeDelay);

        // Load PersistentManager first (additively)
        yield return SceneManager.LoadSceneAsync(persistentManagerScene, LoadSceneMode.Additive);
        Debug.Log("PersistentManager scene loaded.");

        // Load MainMenu (additively)
        yield return SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
        Debug.Log("MainMenu scene loaded additively.");

        // Set MainMenu as the active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenuScene));
        Debug.Log("MainMenu scene set to active.");

        // Unload BootScene (this scene)
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
        Debug.Log("BootScene unloaded.");
    }
}
