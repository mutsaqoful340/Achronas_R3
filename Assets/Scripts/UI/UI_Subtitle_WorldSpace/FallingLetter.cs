using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FallingLetter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeOutDuration = 1.0f; // How long it takes to fade away
    [SerializeField] private LayerMask groundLayer; // Assign your "Ground" layer in inspector!
    [SerializeField] private float flattenDuration = 0.3f; // How fast it snaps flat on landing
    [SerializeField] private float failsafeTimeout = 5.0f; // Extra time before destroying letters that never hit ground
    
    // --- NEW VARIABLE ---
    [SerializeField] private float groundOffset = 0.01f; // Extra distance to sit above the ground

    private TMP_Text tmpText;
    private Camera mainCamera;
    private bool isFalling = false;
    private float maxTorque;

    private float destroyDelay;
    private float fallDelay;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        mainCamera = Camera.main;
    }

    // Update() and FaceCamera() are commented out, which is fine.

    public void StartLifecycle(float fallDelay, float destroyDelay, float torque)
    {
        this.maxTorque = torque;
        this.destroyDelay = destroyDelay; 
        this.fallDelay = fallDelay;       

        StartCoroutine(FallAfterDelay());
    }

    private IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(this.fallDelay);
        DropLetter();
        StartCoroutine(DestroyAfterTimeout());
    }

    private void DropLetter()
    {
        // Physics and unparenting are now handled by PerLetterSubtitleManager
        // Just set the falling flag and add collider for ground detection
        isFalling = true;
        gameObject.AddComponent<BoxCollider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        int collisionLayer = 1 << collision.gameObject.layer;
        if (isFalling && (collisionLayer & groundLayer.value) != 0)
        {
            isFalling = false; 

            Vector3 impactPoint = collision.contacts[0].point;
            StartCoroutine(LandAndFade(impactPoint.y));
        }
    }

    /// <summary>
    /// MODIFIED: This coroutine now uses the collider's HEIGHT and adds the groundOffset.
    /// </summary>
    private IEnumerator LandAndFade(float groundY) 
    {
        // 1. Get collider height *before* destroying it
        BoxCollider bc = GetComponent<BoxCollider>();
        
        // --- THIS IS THE FIX ---
        // Get the collider's local Y-size (its "height") and divide by 2
        float halfHeight = (bc != null) ? bc.size.y / 2.0f : 0.05f;
        // --- END OF FIX ---

        // 2. Destroy physics components
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());
        if (bc != null) Destroy(bc);

        // 3. Calculate start/end states for animation
        Quaternion startRot = transform.rotation;
        Vector3 startPos = transform.position;

        float randomY = Random.Range(0f, 360f); 
        Quaternion endRot = Quaternion.Euler(90, randomY, 0);

        // --- MODIFIED LINE ---
        // The target position:
        // Set Y to the ground level + half the letter's height + your new offset.
        Vector3 endPos = new Vector3(startPos.x, groundY + halfHeight + groundOffset, startPos.z);
        // --- END MODIFICATION ---

        // 4. Animate both position and rotation
        float timer = 0;
        while (timer < flattenDuration)
        {
            timer += Time.deltaTime;
            float t = timer / flattenDuration;
            float smoothT = t * t * (3f - 2f * t); // SmoothStep

            transform.rotation = Quaternion.Slerp(startRot, endRot, smoothT);
            transform.position = Vector3.Lerp(startPos, endPos, smoothT); 
            yield return null;
        }
        
        transform.rotation = endRot;
        transform.position = endPos; 

        // 5. Wait for the 'stay on ground' duration
        float stayOnGroundTime = this.destroyDelay - fadeOutDuration;
        if (stayOnGroundTime > 0)
        {
            yield return new WaitForSeconds(stayOnGroundTime);
        }

        // 6. Fade out
        yield return StartCoroutine(FadeOut());

        // 7. Clean up
        Destroy(gameObject);
    }
    
    // (Rest of the script is the same...)

    private IEnumerator DestroyAfterTimeout()
    {
        yield return new WaitForSeconds(this.fallDelay + this.destroyDelay + failsafeTimeout);
        if (isFalling)
        {
            yield return StartCoroutine(FadeOut());
            Destroy(gameObject);
        }
    }

    private IEnumerator FadeOut()
    {
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());
        if (GetComponent<BoxCollider>()) Destroy(GetComponent<BoxCollider>());

        Color originalColor = tmpText.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        float timer = 0;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            tmpText.color = Color.Lerp(originalColor, targetColor, timer / fadeOutDuration);
            yield return null;
        }
    }
}