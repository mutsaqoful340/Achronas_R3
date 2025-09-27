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
    }

    public void SetIsSaveGame(bool isSaveGame)
    {
        performSaveGame = isSaveGame;
    }

    public void SetIsLoadGame(bool isLoadGame)
    {
        performLoadGame = isLoadGame;
    }

    public void System_ActivateLoadingScreen()
    {
        StartCoroutine(UniversalLoadingScreenCoroutine());
        isLoadingActive = true;
        isTimeBufferOver = false;
    }

    private IEnumerator UniversalLoadingScreenCoroutine()
    {
        // 1. Setup and Start Load Operation
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive);
        
        // 🔥 CRITICAL STEP: Prevent the new scene from activating automatically at 90%
        operation.allowSceneActivation = false; 
        
        float startTime = Time.time;

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true); // Ensure the screen is active immediately
        }

        // 2. Wait for Actual Scene Load Progress (0% to 90%)
        // The load technically completes at 90%, but activation is blocked by allowSceneActivation = false.
        while (operation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBarFill != null)
            {
                loadingBarFill.fillAmount = progressValue;
            }
            yield return null;
        }
        
        // Now, operation.progress is >= 0.9f. The scene data is ready in memory.
        
        // 3. Ensure Minimum Load Time Has Elapsed (The Time Buffer)
        float timeSpent = Time.time - startTime;
        float timeRemaining = minimumLoadTime - timeSpent;

        if (timeRemaining > 0)
        {
            yield return new WaitForSeconds(timeRemaining);
        }
        
        // 4. Final Progress Animation (Finishing to 100%)
        if (loadingBarFill != null)
        {
            float smoothTime = 0.5f;
            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / smoothTime;
                loadingBarFill.fillAmount = Mathf.Lerp(loadingBarFill.fillAmount, 1f, t);
                yield return null;
            }
            loadingBarFill.fillAmount = 1f;
        }

        // --- FINAL TRANSITION (ALL WAITS ARE COMPLETE) ---
        operation.allowSceneActivation = true;
        //if (System_DataPersistentManager.instance != null)
        //{
        //    System_DataPersistentManager.instance.RefreshDataPersistenceObjects();
        //    if (performSaveGame)
        //    {
        //        System_DataPersistentManager.instance.SaveGame();
        //        Debug.Log("Game Saved after scene load.");
        //    }

        //    if (performNewGame)
        //    {
        //        System_DataPersistentManager.instance.NewGame();
        //        Debug.Log("New Game Initialized after scene load.");
        //    }

        //    if (performLoadGame)
        //    {
        //        System_DataPersistentManager.instance.LoadGame();
        //        Debug.Log("Continue Game Loaded after scene load.");
        //    }
        //}
        //else
        //{
            // If the manager is null, log a critical error but continue the scene swap.
            // The game state might be incorrect, but it won't crash.
        //    Debug.LogError("System_DataPersistentManager.instance is NULL! NewGame/LoadGame logic skipped.");
        //}
        // 5. Unload the Old Scene and Wait for Completion
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneToUnloadName);
        while (unloadOperation != null && !unloadOperation.isDone)
        {
            yield return null;
        }

        // 6. Set Active Scene and VISUALLY ACTIVATE THE NEW SCENE
        // This happens instantly after the Unload is complete (or instantly if there was no unload wait).
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoadName));


        // 7. Deactivate Loading Screen (Last Step)
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
        
        isLoadingActive = false; // Cleanup flag
    }
}