using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class System_UniversalLoadingScreen : MonoBehaviour
{
    // A clear, defined state for what the loading screen should do.
    public enum LoadOperation
    {
        None,
        NewGame,
        LoadGame,
        SaveGame,
        TransitionOnly
    }

    public static System_UniversalLoadingScreen instance { get; private set; }

    [Header("Loading Screen UI")]
    public GameObject loadingScreen;
    public UnityEngine.UI.Image loadingBarFill;
    
    [Header("Loading Settings")]
    public float minimumLoadTime = 1.5f;

    // We use the enum instead of multiple booleans
    private LoadOperation currentOperation = LoadOperation.None;
    public string sceneToLoadName;
    public string sceneToUnloadName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- PUBLIC METHODS TO START THE LOADING PROCESS ---

    public void LoadNewGame(string sceneToLoad, string sceneToUnload)
    {
        SetupAndStart(LoadOperation.NewGame, sceneToLoad, sceneToUnload);
    }

    public void LoadSavedGame(string sceneToLoad, string sceneToUnload)
    {
        // The LoadGame call is now handled inside the coroutine at the correct time.
        SetupAndStart(LoadOperation.LoadGame, sceneToLoad, sceneToUnload);
    }
    
    public void TransitionToScene(string sceneToLoad, string sceneToUnload)
    {
        SetupAndStart(LoadOperation.TransitionOnly, sceneToLoad, sceneToUnload);
    }

    private void SetupAndStart(LoadOperation operation, string toLoad, string toUnload)
    {
        currentOperation = operation;
        sceneToLoadName = toLoad;
        sceneToUnloadName = toUnload;
        StartCoroutine(UniversalLoadingScreenCoroutine());
    }

    private IEnumerator UniversalLoadingScreenCoroutine()
    {
        // 1. Activate the loading screen UI
        if (loadingScreen != null) loadingScreen.SetActive(true);
        if (loadingBarFill != null) loadingBarFill.fillAmount = 0;

        // 2. Start loading the new scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        // 3. Update the progress bar until the scene is almost ready
        while (operation.progress < 0.9f)
        {
            loadingBarFill.fillAmount = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        loadingBarFill.fillAmount = 1f;
        operation.allowSceneActivation = true;

        // Wait for the minimum load time to ensure the screen doesn't just flash
        yield return new WaitForSeconds(minimumLoadTime);

        // 4. Set the new scene as active (this is a crucial step!)
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoadName));

        // 5. ---- PERFORM DATA OPERATIONS NOW THAT THE SCENE IS ACTIVE ----
        if (currentOperation == LoadOperation.NewGame)
        {
            System_DataPersistenceManager.instance.NewGame();
        }
        else if (currentOperation == LoadOperation.LoadGame)
        {
            System_DataPersistenceManager.instance.LoadGame(); // CORRECT TIMING!
        }
        else if (currentOperation == LoadOperation.SaveGame)
        {
            System_DataPersistenceManager.instance.SaveGame();
        }
        // If 'TransitionOnly', we do nothing with the data.

        // 6. Unload the old scene
        if (!string.IsNullOrEmpty(sceneToUnloadName))
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneToUnloadName);
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }

        // 7. Deactivate the loading screen UI
        if (loadingScreen != null) loadingScreen.SetActive(false);

        // 8. Reset the state for the next time we use the loading screen
        currentOperation = LoadOperation.None;
    }
}