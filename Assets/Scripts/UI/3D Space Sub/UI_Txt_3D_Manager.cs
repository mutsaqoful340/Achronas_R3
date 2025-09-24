using UnityEngine;
using TMPro;
using System.Collections;

public class UI_Txt_3D_Manager : MonoBehaviour
{
    public GameObject letterPrefab;    // TMP (3D) prefab
    public Transform letterParent;    // parent object for spawned letter
    public float spawnDelay = 0.05f;   // delay per letter
    public TMP_Text layoutText;        // hidden TMP (3D or UGUI) for layout only

    public void ShowSubtitle(string text)
    {
        StartCoroutine(SpawnLetters(text));
    }

    IEnumerator SpawnLetters(string text)
    {
        layoutText.text = text;
        layoutText.ForceMeshUpdate();

        // Use LetterParent's rotation
        Quaternion spawnRotation = letterParent != null ? letterParent.transform.rotation : Quaternion.identity;

        for (int i = 0; i < layoutText.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = layoutText.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            // Character center position relative to layout TMP
            Vector3 charLocalPos = (charInfo.bottomLeft + charInfo.topRight) / 2;
            Vector3 charWorldPos = layoutText.transform.TransformPoint(charLocalPos);

            // Adjust position: small upward offset so letters "fall"
            Vector3 spawnPos = charWorldPos + Vector3.up * 2f;

            // Spawn letter prefab with parent’s rotation
            GameObject letterObj = Instantiate(letterPrefab, spawnPos, spawnRotation);

            // Parent it
            if (letterParent != null)
                letterObj.transform.SetParent(letterParent.transform, true);

            // Assign letter text
            TextMeshPro tmp = letterObj.GetComponent<TextMeshPro>();
            tmp.text = text[i].ToString();

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
