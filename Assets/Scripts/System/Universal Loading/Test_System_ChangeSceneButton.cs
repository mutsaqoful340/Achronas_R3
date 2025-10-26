using UnityEngine;

public class Test_System_ChangeSceneButton : MonoBehaviour
{
    public void OnClick_ChangeScene(string sceneName)
    {
        Test_System_LoadingScreen.instance.LoadScene(sceneName);
    }
}