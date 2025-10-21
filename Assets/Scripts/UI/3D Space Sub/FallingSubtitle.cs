using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class FallingSubtitle : MonoBehaviour
{
    // --- INSPECTOR SETTINGS ---
    [Header("Animation Timings")]
    [Tooltip("How fast letters appear (letters per second)")]
    public float revealSpeed = 20f;

    [Tooltip("How long to wait after the text is fully revealed before it falls")]
    public float fallDelay = 2.0f;

    [Header("Fall Animation")]
    [Tooltip("The downward speed of the letters")]
    public float fallSpeed = 5.0f;

    [Tooltip("How much the letters will randomly rotate as they fall")]
    public float fallRotationSpeed = 100.0f;

    [Tooltip("Adds a bit of random horizontal drift to the fall")]
    public float fallWobble = 0.5f;


    // --- PRIVATE VARIABLES ---
    private TextMeshPro textMeshPro;
    private TMP_TextInfo textInfo;
    private Vector3[][] initialVertexPositions; // To store original positions
    private bool isRevealing = false;
    private bool isFalling = false;


    void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        // Hide the text initially until we want to show it
        textMeshPro.maxVisibleCharacters = 0;
    }

    /// <summary>
    /// Public method to start the subtitle animation sequence.
    /// </summary>
    /// <param name="text">The text you want to display.</param>
    public void DisplayText(string text)
    {
        // Stop any previous animations running on this object
        StopAllCoroutines();

        // Reset state
        textMeshPro.text = text;
        isRevealing = false;
        isFalling = false;

        // Force the mesh to update so we can get its info
        textMeshPro.ForceMeshUpdate();
        textInfo = textMeshPro.textInfo;
        CacheInitialVertexPositions();
        
        // Start the animation sequence
        StartCoroutine(AnimateSubtitle());
    }

    private void CacheInitialVertexPositions()
    {
        int characterCount = textInfo.characterCount;
        initialVertexPositions = new Vector3[characterCount][];

        for (int i = 0; i < characterCount; i++)
        {
            // Skip invisible characters like spaces
            if (!textInfo.characterInfo[i].isVisible)
            {
                initialVertexPositions[i] = null;
                continue;
            }

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            initialVertexPositions[i] = new Vector3[4];
            initialVertexPositions[i][0] = textInfo.meshInfo[materialIndex].vertices[vertexIndex + 0];
            initialVertexPositions[i][1] = textInfo.meshInfo[materialIndex].vertices[vertexIndex + 1];
            initialVertexPositions[i][2] = textInfo.meshInfo[materialIndex].vertices[vertexIndex + 2];
            initialVertexPositions[i][3] = textInfo.meshInfo[materialIndex].vertices[vertexIndex + 3];
        }
    }


    private IEnumerator AnimateSubtitle()
    {
        // --- 1. Reveal Phase ---
        isRevealing = true;
        int totalVisibleCharacters = textInfo.characterCount;
        int visibleCount = 0;
        textMeshPro.maxVisibleCharacters = 0;

        while (visibleCount < totalVisibleCharacters)
        {
            visibleCount = Mathf.CeilToInt(Time.timeSinceLevelLoad * revealSpeed);
            textMeshPro.maxVisibleCharacters = visibleCount;
            yield return null;
        }

        // Ensure all text is visible at the end
        textMeshPro.maxVisibleCharacters = totalVisibleCharacters;
        isRevealing = false;

        // --- 2. Wait Phase ---
        yield return new WaitForSeconds(fallDelay);

        // --- 3. Fall Phase ---
        isFalling = true;

        // We can let the Update method handle the falling animation from here
        // Optional: Hide after a certain time
        yield return new WaitForSeconds(5.0f); // Fall for 5 seconds
        gameObject.SetActive(false); // Then hide the object
    }


    void Update()
    {
        if (!isFalling)
        {
            return; // Do nothing if we are not in the falling state
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            
            // Skip invisible characters
            if (!charInfo.isVisible)
            {
                continue;
            }

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] sourceVertices = initialVertexPositions[i];
            Vector3[] destinationVertices = cachedMeshInfo[materialIndex].vertices;

            // Calculate the center of the character
            Vector3 center = (sourceVertices[0] + sourceVertices[2]) / 2f;

            // Create the transformation matrix for rotation and movement
            float wobble = Mathf.PerlinNoise(i, Time.time) - 0.5f; // Unique wobble per character
            Vector3 fallVector = new Vector3(wobble * fallWobble, -fallSpeed, 0) * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(0, 0, fallRotationSpeed * Time.deltaTime);

            // Apply transformation to each vertex of the character
            for (int j = 0; j < 4; j++)
            {
                Vector3 originalVertex = destinationVertices[vertexIndex + j];
                Vector3 rotatedVertex = rotation * (originalVertex - center) + center;
                destinationVertices[vertexIndex + j] = rotatedVertex + fallVector;
            }
        }
        
        // Push the updated vertex data to the mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = cachedMeshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}