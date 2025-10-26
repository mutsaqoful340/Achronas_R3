using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Test_System_LoadingScreen : MonoBehaviour
{
    public static Test_System_LoadingScreen instance;

    [SerializeField] private GameObject _loadingScreenUI;
    [SerializeField] private Image _progressBarImage;
    private float _fillTarget;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        _loadingScreenUI.SetActive(false);
    }

    public async void LoadScene(string sceneName)
    {
        _fillTarget = 0;
        _progressBarImage.fillAmount = 0;
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        _loadingScreenUI.SetActive(true);

        System_DataPersistenceManager.instance.GameSystem();
        Debug.Log("Calling Game System for scene load: " + sceneName);

        do
        {
            await Task.Delay(100);
            _fillTarget = asyncLoad.progress / 0.9f;
        }
        while (asyncLoad.progress < 0.9f);

        await Task.Delay(1000);

        asyncLoad.allowSceneActivation = true;

        await Task.Delay(500);
        _loadingScreenUI.SetActive(false);
    }

    void Update()
    {
        _progressBarImage.fillAmount = Mathf.MoveTowards(_progressBarImage.fillAmount, _fillTarget, 3 * Time.deltaTime);
    }
}
