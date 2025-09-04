using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_SceneLoader : MonoBehaviour
{
    [Header("Scene to Load")]
    public string sceneToLoad;

    [Header("Scene to Unload")]
    public string sceneToUnload;

    public void SwitchScenes()
    {
        SceneManager.UnloadSceneAsync(sceneToUnload);
        Debug.Log($"{sceneToUnload} scene unloaded.");

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        Debug.Log($"{sceneToLoad} scene loaded.");
    }
}
