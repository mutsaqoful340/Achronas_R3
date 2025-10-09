using UnityEngine;
using UnityEngine.UI;

public class Game_PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float minStaminaToRun = 10f; // Minimum stamina required to start running
    public float staminaRegenRate = 5f; // Stamina points regenerated per second
    public float staminaDepletionRate = 10f; // Stamina points depleted per second when running
    public Game_QTEController qteController; // Reference to the QTE controller for recovery

    [Header("Stamina UI")]
    public Image staminaBar;

    public bool CanRun { get; private set; } = true;
    private bool isExhausted = false; // Internal state to track exhaustion

    private Player_InputHandle playerInput;

    void Start()
    {
        currentStamina = maxStamina;
        UpdateStaminaDisplay();
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
        
        // Once we have the playerInput reference, we can proceed with the normal logic.
        HandleStamina();
    }

    void HandleStamina()
    {
        bool staminaChanged = false;

        // If player is exhausted, check if they have recovered enough stamina to run again
        if (isExhausted && currentStamina >= minStaminaToRun)
        {
            isExhausted = false;
        }

        // The player can run if they are not exhausted
        CanRun = !isExhausted;

        // Deplete stamina only if the run button is held AND the player is allowed to run
        if (playerInput.RunHeld && CanRun && currentStamina > 0)
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            staminaChanged = true;
            qteController.StartQTE(); // Start the QTE when running

            // If stamina runs out, the player becomes exhausted
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
            }
        }
        // Regenerate stamina if it's not full
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            staminaChanged = true;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }

        // Only update the display if the stamina value has changed
        if (staminaChanged)
        {
            UpdateStaminaDisplay();
        }
    }

    private void UpdateStaminaDisplay()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }
}
