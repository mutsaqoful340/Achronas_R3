using UnityEngine;

public class Game_HeartBeat : MonoBehaviour
{
    public float beatInterval = 1.0f; // Time between beats in seconds
    //public AudioClip heartBeatSound; // Sound to play on each beat
    //private AudioSource audioSource;
    private float timer;
    private Player_InputHandle playerInput;

    void Start()
    {
        //audioSource = gameObject.AddComponent<AudioSource>();
        //audioSource.clip = heartBeatSound;
        //audioSource.loop = false;
        timer = 0f;
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void Update()
    {
        // If we don't have a reference to the player's input yet, try to find it.
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<Player_InputHandle>();

            // If we still couldn't find it (player hasn't spawned), exit the Update for this frame.
            // This prevents errors and we'll try again on the next frame.
            if (playerInput == null)
            {
                return;
            }
        }
        timer += Time.deltaTime;
        if (timer >= beatInterval)
        {
            PlayHeartBeat();
            timer = 0f;
        }
    }

    void PlayHeartBeat()
    {
        //if (heartBeatSound != null)
        //{
        //    audioSource.PlayOneShot(heartBeatSound);
        //}

        Debug.Log("Heart Beat");

        if (playerInput.InteractPressed)
        {
            
        }
    }
}
