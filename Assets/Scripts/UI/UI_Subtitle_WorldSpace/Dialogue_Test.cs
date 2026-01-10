using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Test : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    
    [Header("Dialogue Options")]
    [Tooltip("Use ScriptableObject for easy editing")]
    [SerializeField] private DialogueSequenceSO dialogueSequence;
    
    [Header("Or Use Manual Setup")]
    [SerializeField] private PerLetterSubtitleManager subtitleManager;
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
        if (dialogueManager == null)
        {
            Debug.LogError("Dialogue_Test: DialogueManager not assigned!");
            return;
        }

        // Use ScriptableObject (recommended)
        if (!useManualDialogue && dialogueSequence != null)
        {
            dialogueManager.PlayDialogue(dialogueSequence);
        }
        // Or use manual code-based dialogue
        else if (useManualDialogue && subtitleManager != null)
        {
            TestManualDialogue();
        }
        else
        {
            Debug.LogError("Dialogue_Test: No dialogue configured! Assign a DialogueSequence or enable useManualDialogue with a SubtitleManager.");
        }
    }

    private void TestManualDialogue()
    {
        // Create a test dialogue sequence manually (for advanced users)
        var actions = new List<IDialogueAction>
        {
            new ShowTextAction(subtitleManager, "Hello there, traveler!"),
            new WaitAction(this, 1.5f),
            new ShowTextAction(subtitleManager, "Welcome to this world."),
            new WaitAction(this, 1.0f),
            new ShowTextAction(subtitleManager, "May your journey be safe."),
            new TriggerEventAction(() => Debug.Log("Dialogue Test Complete!"))
        };

        dialogueManager.PlayDialogue(actions);
    }
}

