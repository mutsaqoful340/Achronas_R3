using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Test : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueSystem dialogueSystem;
    
    [Header("Dialogue Options")]
    [Tooltip("Use ScriptableObject for easy editing")]
    [SerializeField] private DialogueSequenceSO dialogueSequence;
    
    [Header("Or Use Manual Setup")]
    [SerializeField] private SubtitleSystem subtitleSystem;
    [SerializeField] private bool useManualDialogue = false;

    [Header("Settings")]
    [SerializeField] private bool playOnTrigger = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playOnTrigger)
        {
            TestDialogue();
        }
    }

    [ContextMenu("Test Dialogue")]
    public void TestDialogue()
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("Dialogue_Test: DialogueSystem not assigned!");
            return;
        }

        // Use ScriptableObject (recommended)
        if (!useManualDialogue && dialogueSequence != null)
        {
            dialogueSystem.PlayDialogue(dialogueSequence);
        }
        // Or use manual code-based dialogue
        else if (useManualDialogue && subtitleSystem != null)
        {
            TestManualDialogue();
        }
        else
        {
            Debug.LogError("Dialogue_Test: No dialogue configured! Assign a DialogueSequence or enable useManualDialogue with a SubtitleSystem.");
        }
    }

    private void TestManualDialogue()
    {
        // Create a test dialogue sequence manually (for advanced users)
        var actions = new List<DialogueSystem.IDialogueAction>
        {
            new DialogueSystem.ShowTextAction(subtitleSystem, "Hello there, traveler!"),
            new DialogueSystem.WaitAction(this, 1.5f),
            new DialogueSystem.ShowTextAction(subtitleSystem, "Welcome to this world."),
            new DialogueSystem.WaitAction(this, 1.0f),
            new DialogueSystem.ShowTextAction(subtitleSystem, "May your journey be safe."),
            new DialogueSystem.TriggerEventAction(() => Debug.Log("Dialogue Test Complete!"))
        };

        dialogueSystem.PlayDialogue(actions);
    }
}

