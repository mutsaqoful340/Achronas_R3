using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // We need this to set the text on the prefab

// This script now requires a TMP_Text component on the *same* GameObject
[RequireComponent(typeof(TMP_Text))]
public class PerLetterSubtitleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject letterPrefab; // Drag your 'LetterPrefab' here!

    [Header("Appearance & Timing")]
    [SerializeField] private float perLetterRevealDelay = 0.05f; // Time between each letter *appearing*
    [SerializeField] private float fallDelayAfterReveal = 1.5f; // How long a letter stays up *before* it falls
    [SerializeField] private float destroyDelayAfterFall = 4.0f; // How long it stays on the ground *before fading*

    [Header("Physics")]
    // We no longer need letterSpacing!
    [SerializeField] private float maxRotationTorque = 20f;

    [Header("Input References")]
    public Player_InputHandle inputHandler; // Your input handler

    // Reference to the invisible "layout guide" text component
    private TMP_Text layoutGuideText;
    private bool isSubtitleActive = false; // Prevents spamming new subtitles

    void Awake()
    {
        // Get the layout guide component
        inputHandler = FindAnyObjectByType<Player_InputHandle>();
        layoutGuideText = GetComponent<TMP_Text>();
        if (layoutGuideText == null)
        {
            Debug.LogError("PerLetterSubtitleManager needs a TMP_Text component on the same GameObject!");
        }
        // Ensure it's invisible (you should also set this in the inspector)
        layoutGuideText.color = new Color(0, 0, 0, 0); 
    }

    // --- Example of how to call it ---
    void Update()
    {
        // I noticed your original script had two identical 'if' blocks.
        // That would start two subtitles at the same time.
        // Here's a better way to test, using a bool to prevent spamming.
        if (inputHandler.InteractPressed && !isSubtitleActive)
        {
            // You can alternate between test strings or use one.
            StartSubtitle("This is a test with proportional spacing!");
        }
    }

    void LateUpdate()
    {
        // Make the subtitle manager always face the camera
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
    // --- End Example ---

    /// <summary>
    /// Call this from any other script to start a new subtitle.
    /// </summary>
    public void StartSubtitle(string text, System.Action onComplete = null)
    {
        if (isSubtitleActive) return; // Don't start a new one if one is running
        StartCoroutine(SpawnLetters(text, onComplete));
    }

    private IEnumerator SpawnLetters(string text, System.Action onComplete = null)
    {
        isSubtitleActive = true;

        // List to store all spawned letters
        List<GameObject> spawnedLetters = new List<GameObject>();

        // 1. Set the text on the invisible layout guide
        layoutGuideText.text = text;

        // 2. IMPORTANT: Force it to update its layout data immediately
        layoutGuideText.ForceMeshUpdate();

        // 3. Get the calculated layout data
        TMP_TextInfo textInfo = layoutGuideText.textInfo;

        // Loop through every *visible* character in the text
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            // Get the data for this specific character
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Skip invisible characters (like spaces)
            if (!charInfo.isVisible)
            {
                continue;
            }

            // 4. Wait to "reveal" this letter
            yield return new WaitForSeconds(perLetterRevealDelay);

            // --- THIS IS THE FIX ---

            // 5. Calculate the letter's local horizontal center
            float midX = (charInfo.vertex_BL.position.x + charInfo.vertex_BR.position.x) / 2f;

            // 6. Create the local position.
            // We use midX for proportional horizontal spacing.
            // We use 0 for Y because the manager's transform is *already*
            // at the vertical middle of the text (due to Middle alignment).
            // This ensures all letters share the same vertical center.
            Vector3 localPos = new Vector3(midX, 0, 0);

            // 7. Convert the local position to a world position
            Vector3 worldPos = transform.TransformPoint(localPos);
            
            // --- END OF FIX ---

            // 8. Instantiate and position the letter with current manager rotation
            GameObject letterInstance = Instantiate(letterPrefab, worldPos, this.transform.rotation);

            // Parent it to the manager so it follows any movement
            letterInstance.transform.SetParent(this.transform);

            // Make Rigidbody kinematic so it doesn't fall while parented
            Rigidbody rb = letterInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // 9. Set its text (to just this one character)
            TMP_Text tmpText = letterInstance.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = charInfo.character.ToString();
            }

            // Store the letter for later
            spawnedLetters.Add(letterInstance);
        }

        // Wait for the delay before all letters fall together
        yield return new WaitForSeconds(fallDelayAfterReveal);

        // Unparent all letters and trigger them to fall simultaneously
        foreach (GameObject letter in spawnedLetters)
        {
            if (letter != null)
            {
                // Capture world transform BEFORE unparenting
                Vector3 worldPos = letter.transform.position;
                Quaternion worldRot = letter.transform.rotation;

                // Unparent the letter so it no longer follows the manager
                letter.transform.SetParent(null);

                // Explicitly restore world transform to prevent any snap
                letter.transform.position = worldPos;
                letter.transform.rotation = worldRot;

                // Enable physics and add random torque
                Rigidbody rb = letter.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Enable physics
                    rb.mass = 0.5f;
                    
                    // Clear any existing velocities to prevent snapping
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    
                    Vector3 randomTorque = new Vector3(
                        Random.Range(-maxRotationTorque, maxRotationTorque),
                        Random.Range(-maxRotationTorque, maxRotationTorque),
                        Random.Range(-maxRotationTorque, maxRotationTorque)
                    );
                    rb.AddTorque(randomTorque, ForceMode.Impulse);
                }

                // Trigger its lifecycle (with 0 delay since we already waited)
                FallingLetter letterScript = letter.GetComponent<FallingLetter>();
                if (letterScript != null)
                {
                    letterScript.StartLifecycle(0f, destroyDelayAfterFall, maxRotationTorque);
                }
            }
        }

        // --- Cleanup ---
        // Wait for the letters to be destroyed
        yield return new WaitForSeconds(destroyDelayAfterFall + 1.0f); 
        isSubtitleActive = false; // Allow a new subtitle to be triggered
        
        // Notify completion
        onComplete?.Invoke();
    }
}