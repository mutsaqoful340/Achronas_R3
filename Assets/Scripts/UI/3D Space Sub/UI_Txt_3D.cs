using UnityEngine;
using TMPro;
using System.Collections;

public class UI_Txt_3D : MonoBehaviour
{
    [Header("Timing")]
    public float waitBeforeFall = 1f;
    public float fallSpeed = 2f;
    public float lifetimeAfterLanding = 2f;
    public float fadeDuration = 0.5f;

    [Header("Rotation Control")]
    public float fallRotationSpeed = 180f; // spin speed while falling
    public float maxLandingTilt = 45f;     // random tilt around Z when settled

    private bool hasLanded = false;
    private bool _isFalling = false;

    private TextMeshPro text;   // ✅ TextMeshPro (3D), not UGUI
    private float groundY;

    // Random fall spin
    private float randomFallRotation;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();

        // Raycast straight down to detect ground
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            groundY = hit.point.y;
        }
        else
        {
            groundY = 0f;
        }

        // random spin direction (-1 or 1)
        randomFallRotation = Random.Range(0, 2) == 0 ? -1f : 1f;

        // start with wait delay
        StartCoroutine(StartFallingAfterDelay());
    }

    IEnumerator StartFallingAfterDelay()
    {
        yield return new WaitForSeconds(waitBeforeFall);
        _isFalling = true;
        transform.SetParent(null, true);
    }

    void Update()
    {
        if (_isFalling && !hasLanded)
        {
            // Falling in world space
            Vector3 pos = transform.position;
            pos.y -= fallSpeed * Time.deltaTime;
            transform.position = pos;

            // Spin while falling
            transform.Rotate(Vector3.forward, randomFallRotation * fallRotationSpeed * Time.deltaTime);

            // Landing check
            if (transform.position.y <= groundY)
            {
                transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
                hasLanded = true;

                // Lay flat face-up
                float finalZRot = Random.Range(-maxLandingTilt, maxLandingTilt);
                transform.rotation = Quaternion.Euler(90f, 0f, finalZRot);

                StartCoroutine(DisappearAfterDelay());
            }
        }
    }

    IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(lifetimeAfterLanding);

        float t = 0f;
        Color c = text.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            text.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
}
