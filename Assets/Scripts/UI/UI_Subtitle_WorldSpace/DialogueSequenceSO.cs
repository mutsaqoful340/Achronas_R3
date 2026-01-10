using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that stores a sequence of dialogue lines.
/// Create via: Right-click in Project -> Create -> Dialogue System -> Dialogue Sequence
/// </summary>
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Sequence", order = 1)]
public class DialogueSequenceSO : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)]
        public string text;
        
        [Tooltip("How long to wait after this line finishes before continuing")]
        public float delayAfter = 0.5f;
    }

    [Header("Dialogue Content")]
    public List<DialogueLine> lines = new List<DialogueLine>();

    [Header("Settings")]
    [Tooltip("Optional: Play this sound when dialogue starts")]
    public AudioClip startSound;
    
    [Tooltip("Optional: Play this sound when dialogue ends")]
    public AudioClip endSound;

    private void OnValidate()
    {
        // Ensure at least one line exists when created
        if (lines == null || lines.Count == 0)
        {
            lines = new List<DialogueLine>
            {
                new DialogueLine { text = "New dialogue line...", delayAfter = 0.5f }
            };
        }
    }
}
