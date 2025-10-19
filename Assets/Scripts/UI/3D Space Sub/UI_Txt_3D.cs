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
    private bool isFalling = false;

    private TextMeshProUGUI text;
    private RectTransform rect;
    private float groundY;

    // Random fall spin
    private float randomFallRotation;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();

        // Raycast straight down to detect ground
        Ray ray = new Ray(rect.position, Vector3.down);
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
        isFalling = true;
    }

    void Update()
    {
        if (isFalling && !hasLanded)
        {
            // Falling
            Vector3 pos = rect.localPosition;
            pos.y -= fallSpeed * Time.deltaTime;
            rect.localPosition = pos;

            // Spin while falling
            rect.Rotate(Vector3.forward, randomFallRotation * fallRotationSpeed * Time.deltaTime);

            // Landing check
            if (rect.position.y <= groundY)
            {
                rect.position = new Vector3(rect.position.x, groundY, rect.position.z);
                hasLanded = true;

                // Settle face-up like on the ground, only Z tilt random
                float finalZRot = Random.Range(-maxLandingTilt, maxLandingTilt);
                rect.localRotation = Quaternion.Euler(90f, 0f, finalZRot); 
                // 90° on X = facing up, Z = tilt variation

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
