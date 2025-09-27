using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_Button_SceneLoader : MonoBehaviour
{
    [Header("Loading Screen UI")]
    public GameObject LoadingScreen;
    public UnityEngine.UI.Image LoadingBarFill;

    [Header("Scene to Load")]
    public string sceneToLoad;

    [Header("Scene to Unload")]
    public string sceneToUnload;

    public void SwitchScenes()
    {
        StartCoroutine(SwitchScenesAsync());
    }

    private IEnumerator SwitchScenesAsync()
    {
        // Start loading the new scene
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        
        if (LoadingScreen != null)
            LoadingScreen.SetActive(true);

        while (!loadOp.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOp.progress / 0.9f);
            if (LoadingBarFill != null)
                LoadingBarFill.fillAmount = progressValue;
            yield return null;
        }

        // Set active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
        Debug.Log($"{sceneToLoad} scene loaded.");

        // Unload old scene
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
        while (!unloadOp.isDone)
            yield return null;

        Debug.Log($"{sceneToUnload} scene unloaded.");

        if (LoadingScreen != null)
            LoadingScreen.SetActive(false);
    }
}
