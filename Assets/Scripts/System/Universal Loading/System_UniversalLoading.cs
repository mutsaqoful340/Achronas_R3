using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class System_UniversalLoadingScreen : MonoBehaviour
{
    public static System_UniversalLoadingScreen instance { get; set; }

    [Header("Loading Screen UI")]
    public GameObject loadingScreen;
    public UnityEngine.UI.Image loadingBarFill;
    
    [Header("Loading Settings")]
    public float minimumLoadTime = 1.5f; // Minimum time (in seconds) the screen must stay visible

    [HideInInspector] public string sceneToLoadName;
    [HideInInspector] public string sceneToUnloadName;
    [HideInInspector] public bool performNewGame = false;
    [HideInInspector] public bool performSaveGame= false;
    [HideInInspector] public bool performLoadGame = false;

    private bool isLoadingActive = false;
    private bool isTimeBufferOver = false;

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

    public void SetIsNewGame(bool isNewGame)
    {
        performNewGame = isNewGame;
        System_ActivateLoadingScreen();
    }

    public void SetIsSaveGame(bool isSaveGame)
    {
        performSaveGame = isSaveGame;
        System_ActivateLoadingScreen();
    }

    public void SetIsLoadGame(bool isLoadGame)
    {
        performLoadGame = isLoadGame;
        System_ActivateLoadingScreen();
    }

    public void System_ActivateLoadingScreen()
    {
        StartCoroutine(UniversalLoadingScreenCoroutine());
        isLoadingActive = true;
        isTimeBufferOver = false;
    }

    private IEnumerator UniversalLoadingScreenCoroutine()
    {
        // Start loading the new scene additively
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false; // Wait until loading bar is full

        // Progress bar update loop
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBarFill != null)
                loadingBarFill.fillAmount = progress;

            yield return null;
        }

        // Force bar to 100%
        if (loadingBarFill != null)
            loadingBarFill.fillAmount = 1f;

        // Allow scene activation after bar is full
        operation.allowSceneActivation = true;

        if (operation.allowSceneActivation)
        {
            if (performNewGame)
            {
                System_DataPersistenceManager.instance.NewGame();

            }
            else if (performLoadGame)
            {
                System_DataPersistenceManager.instance.LoadGame();

            }
            else if (performSaveGame)
            {
                System_DataPersistenceManager.instance.SaveGame();
            }
        }
        // Wait until fully loaded
            while (!operation.isDone)
                yield return null;

        // Unload the old scene
        if (!string.IsNullOrEmpty(sceneToUnloadName))
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneToUnloadName);
            while (!unloadOperation.isDone)
                yield return null;
        }

        // Set the newly loaded scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoadName));

        // Hide the loading screen
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

}