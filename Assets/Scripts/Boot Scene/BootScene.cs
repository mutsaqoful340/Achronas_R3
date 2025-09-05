using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class BootScene : MonoBehaviour
{
    [Header("Boot Time Delay")]
    [SerializeField] private float bootTimeDelay = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(LoadScene_MainMenu), bootTimeDelay);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadScene_MainMenu()
    {
        SceneManager.UnloadSceneAsync("BootScene");
        Debug.Log("BootScene unloaded.");

        SceneManager.LoadScene("PersistentManager", LoadSceneMode.Single);
        Debug.Log("PersistentManager scene loaded.");

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        Debug.Log("MainMenu scene loaded aditively.");
    }
}
