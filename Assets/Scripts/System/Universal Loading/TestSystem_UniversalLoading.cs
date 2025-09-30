using UnityEngine;

public class TestSystem_UniversalLoading : MonoBehaviour
{
    [Header("Target Scene Name")]
    public string sceneToLoadName;
    public string currentActiveSceneName;

    public void LoadTargetScene()
    {
        System_UniversalLoadingScreen.instance.sceneToLoadName = sceneToLoadName;
        //System_UniversalLoadingScreen.instance.SetIsSaveGame(true);
        //System_UniversalLoadingScreen.instance.System_ActivateLoadingScreen();
    }
}