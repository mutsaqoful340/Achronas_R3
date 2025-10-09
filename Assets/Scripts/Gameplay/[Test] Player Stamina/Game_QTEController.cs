using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// An enum to represent the zones. It's cleaner than using strings.
public enum QTEZone { None, Success, GreatSuccess }

public class Game_QTEController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject qteContainer;
    public Image needle;
    public GameObject successZonesParent; // A parent object holding both success zones

    [Header("QTE Settings")]
    public float needleSpeed = 200f;
    public float qteDelay = 1.0f; // Delay before QTE starts
    private float qteTimer;
    public float staminaRecoveryAmount = 20f; // Amount of stamina to recover on success
    public float staminaPenaltyAmount = 10f; // Amount of stamina to lose on failure
    
    [Header("Events")]
    public UnityEvent OnQTESuccess;
    //public UnityEvent OnQTEGreatSuccess;
    public UnityEvent OnQTEFailure;

    [Header("References")]
    public Game_PlayerStamina playerStamina; // Reference to the player's stamina script
    

    // QTE
    private bool qteActive = false;
    private bool qteJustEnded = false;
    private float currentAngle = 0f;
    private float targetAngle = 0f; // Add this line
    private QTEZone currentZone = QTEZone.None; // Tracks which zone the needle is in

    private Player_InputHandle playerInput;

    void Start()
    {
        qteContainer.SetActive(false);
        qteTimer = 0f;
    }

    void Update()
    {
        // Find the player input if we don't have it
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<Player_InputHandle>();
        }
        // If we still can't find it, don't do anything
        if (playerInput == null)
        {
            return;
        }

        // ✅ BLOCK 1: This code only runs WHEN the QTE is active.
        if (qteActive)
        {
            // Rotate the needle
            currentAngle += needleSpeed * Time.deltaTime;
            needle.transform.rotation = Quaternion.Euler(0, 0, -currentAngle);

            // Check for player input
            if (playerInput.InteractPressed)
            {
                CheckSuccess();
            }

            // Check if the needle has completed a full circle
            if (currentAngle >= targetAngle)
            {
                FailQTE();
            }
        }

        // ✅ BLOCK 2: This code only runs AFTER the QTE has just ended.
        if (qteJustEnded)
        {
            // This is your timer, and now it will run!
            qteTimer += Time.deltaTime;
            if (qteTimer >= qteDelay)
            {
                Debug.Log("Cooldown finished. Ready for the next QTE!");
                qteJustEnded = false;
                qteTimer = 0f;
            }
        }
    }
    // Public method for the needle script to update the current zone
    public void SetCurrentZone(QTEZone zone)
    {
        currentZone = zone;
    }

    public void StartQTE()
    {
        if (!qteActive && !qteJustEnded)
        {
            qteContainer.SetActive(true);
            qteActive = true;

            // Set the starting angle to a random value
            currentAngle = Random.Range(0f, 360f); 
            // The QTE will fail after one full rotation from this new starting point
            targetAngle = currentAngle + 360f; 

            currentZone = QTEZone.None; 

            successZonesParent.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        }
    }

    private void CheckSuccess()
    {
        // The success check is now much simpler!
        switch (currentZone)
        {
            //case QTEZone.GreatSuccess:
                //Debug.Log("Great Success!");
                //OnQTEGreatSuccess?.Invoke();
                //break;
            case QTEZone.Success:
                Debug.Log("Success!");
                OnQTESuccess?.Invoke();
                playerStamina.currentStamina += staminaRecoveryAmount; // Recover some stamina on success
                EndQTE();
                break;
            case QTEZone.None: // If it's in no zone, it's a failure
                Debug.Log("Failure! (Pressed at wrong time)");
                OnQTEFailure?.Invoke();
                playerStamina.currentStamina -= staminaPenaltyAmount; // Recover some stamina on success
                EndQTE();
                break;
        }

        EndQTE();
    }

    private void FailQTE()
    {
        Debug.Log("Failure! (Time ran out)");
        OnQTEFailure?.Invoke();
        EndQTE();   
    }

    private void EndQTE()
    {
        qteActive = false;
        qteJustEnded = true;
        qteContainer.SetActive(false);
    }
}