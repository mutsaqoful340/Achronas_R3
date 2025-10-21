using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class DroppingSubtitle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text tmpText;

    [Header("Reveal Settings")]
    [SerializeField] private float revealSpeed = 0.05f; // Time between each letter

    [Header("Drop Settings")]
    [SerializeField] private float dropDelay = 1.0f; // Time to wait after reveal before dropping
    [SerializeField] private float destroyDelay = 5.0f; // Time to wait after dropping to destroy
    [SerializeField] private float maxRotationTorque = 10f; // How much it spins

    private bool isRevealing = false;
    private bool isDropping = false;
    private Camera mainCamera;

    void Awake()
    {
        // Get the component if not assigned
        if (tmpText == null)
        {
            tmpText = GetComponent<TMP_Text>();
        }
        mainCamera = Camera.main;
    }

    void Update()
    {
        // During the reveal phase, make the text always face the camera
        if (isRevealing && !isDropping && mainCamera != null)
        {
            FaceCamera();
        }
    }

    /// <summary>
    /// Makes the text face the main camera.
    /// </summary>
    private void FaceCamera()
    {
        // LookAt points the Z-axis (forward) at the target.
        // TextMeshPro's front face is its Z-axis, so this works.
        // If your text appears backward, uncomment the Rotate line.
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0); 
    }

    /// <summary>
    /// Call this from another script to start the subtitle effect.
    /// </summary>
    /// <param name="newText">The full text to display.</param>
    public void StartSubtitle(string newText)
    {
        // Reset flags and components from any previous run
        isRevealing = true;
        isDropping = false;

        // Ensure no physics components exist yet
        if (GetComponent<Rigidbody>())
        {
            Destroy(GetComponent<Rigidbody>());
        }
        if (GetComponent<BoxCollider>())
        {
            Destroy(GetComponent<BoxCollider>());
        }

        // Set up the text
        tmpText.text = newText;
        tmpText.maxVisibleCharacters = 0; // Hide all text initially
        
        // Start the reveal coroutine
        StartCoroutine(RevealText());
    }

    /// <summary>
    // Coroutine to reveal the text one letter at a time.
    /// </summary>
    private IEnumerator RevealText()
    {
        int totalChars = tmpText.text.Length;

        for (int i = 0; i <= totalChars; i++)
        {
            tmpText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(revealSpeed);
        }

        // Text is fully revealed
        isRevealing = false;

        // Wait a moment before dropping
        yield return new WaitForSeconds(dropDelay);

        // Start the drop phase
        DropText();
    }

    /// <summary>
    /// Adds physics components to make the text drop and tumble.
    /// </summary>
    private void DropText()
    {
        isDropping = true;

        // 1. Add a Rigidbody to enable physics (gravity)
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 1f; // Give it some mass

        // 2. Add a BoxCollider so it can collide with the ground
        // AddComponent<BoxCollider>() will automatically size the
        // collider to fit the text mesh's bounds.
        gameObject.AddComponent<BoxCollider>();

        // 3. Apply a random spin (torque) for the tumbling effect
        float randomX = Random.Range(-maxRotationTorque, maxRotationTorque);
        float randomY = Random.Range(-maxRotationTorque, maxRotationTorque);
        float randomZ = Random.Range(-maxRotationTorque, maxRotationTorque);
        
        rb.AddTorque(new Vector3(randomX, randomY, randomZ));

        // 4. Start the timer to destroy the object after it has fallen
        StartCoroutine(DestroyAfterTime());
    }

    /// <summary>
    /// Destroys this GameObject after a set time.
    /// </summary>
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}