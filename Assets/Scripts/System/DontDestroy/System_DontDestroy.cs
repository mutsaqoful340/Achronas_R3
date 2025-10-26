using UnityEngine;

public class System_DontDestroy : MonoBehaviour
{
    private static System_DontDestroy _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
