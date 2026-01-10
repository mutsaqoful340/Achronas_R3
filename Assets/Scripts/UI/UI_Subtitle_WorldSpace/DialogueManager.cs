using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages dialogue sequences using an action list pattern.
/// Actions execute sequentially, each waiting for the previous to complete.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PerLetterSubtitleManager subtitleManager;
    [SerializeField] private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool logActionExecution = true;

    private List<IDialogueAction> currentActionList;
    private int currentActionIndex = 0;
    private bool isPlayingDialogue = false;

    /// <summary>
    /// Start executing a dialogue action list
    /// </summary>
    public void PlayDialogue(List<IDialogueAction> actions)
    {
        if (isPlayingDialogue)
        {
            Debug.LogWarning("DialogueManager: Already playing dialogue!");
            return;
        }

        if (actions == null || actions.Count == 0)
        {
            Debug.LogWarning("DialogueManager: No actions to execute!");
            return;
        }

        currentActionList = actions;
        currentActionIndex = 0;
        isPlayingDialogue = true;

        if (logActionExecution)
            Debug.Log($"DialogueManager: Starting dialogue with {actions.Count} actions");

        ExecuteNextAction();
    }

    /// <summary>
    /// Start executing a dialogue from a ScriptableObject
    /// </summary>
    public void PlayDialogue(DialogueSequenceSO dialogueSequence)
    {
        if (dialogueSequence == null)
        {
            Debug.LogError("DialogueManager: DialogueSequence is null!");
            return;
        }

        if (dialogueSequence.lines == null || dialogueSequence.lines.Count == 0)
        {
            Debug.LogWarning("DialogueManager: DialogueSequence has no lines!");
            return;
        }

        // Convert ScriptableObject data to action list
        var actions = new List<IDialogueAction>();

        // Optional start sound
        if (dialogueSequence.startSound != null && audioSource != null)
        {
            actions.Add(new PlaySoundAction(audioSource, dialogueSequence.startSound, false));
        }

        // Add each dialogue line
        foreach (var line in dialogueSequence.lines)
        {
            if (!string.IsNullOrEmpty(line.text))
            {
                actions.Add(new ShowTextAction(subtitleManager, line.text));
                
                if (line.delayAfter > 0)
                {
                    actions.Add(new WaitAction(this, line.delayAfter));
                }
            }
        }

        // Optional end sound
        if (dialogueSequence.endSound != null && audioSource != null)
        {
            actions.Add(new PlaySoundAction(audioSource, dialogueSequence.endSound, false));
        }

        // Execute the generated action list
        PlayDialogue(actions);
    }

    /// <summary>
    /// Stop the current dialogue sequence
    /// </summary>
    public void StopDialogue()
    {
        if (logActionExecution)
            Debug.Log("DialogueManager: Dialogue stopped");

        isPlayingDialogue = false;
        currentActionList = null;
        currentActionIndex = 0;
    }

    /// <summary>
    /// Check if dialogue is currently playing
    /// </summary>
    public bool IsPlaying => isPlayingDialogue;

    private void ExecuteNextAction()
    {
        // Check if we're done
        if (currentActionIndex >= currentActionList.Count)
        {
            OnDialogueComplete();
            return;
        }

        // Get next action
        IDialogueAction action = currentActionList[currentActionIndex];
        
        if (logActionExecution)
            Debug.Log($"DialogueManager: Executing action {currentActionIndex + 1}/{currentActionList.Count} ({action.GetType().Name})");

        currentActionIndex++;

        // Execute action with callback to next action
        action.Execute(() => OnActionComplete());
    }

    private void OnActionComplete()
    {
        if (!isPlayingDialogue) return; // Dialogue was stopped

        // Move to next action
        ExecuteNextAction();
    }

    private void OnDialogueComplete()
    {
        if (logActionExecution)
            Debug.Log("DialogueManager: Dialogue sequence complete");

        isPlayingDialogue = false;
        currentActionList = null;
        currentActionIndex = 0;
    }

    // ===== EXAMPLE USAGE =====
    // Call this from another script or button to test

    [ContextMenu("Test Dialogue Sequence")]
    public void TestDialogueSequence()
    {
        var actions = new List<IDialogueAction>
        {
            new ShowTextAction(subtitleManager, "Hello there!"),
            new WaitAction(this, 1.0f),
            new ShowTextAction(subtitleManager, "How are you today?"),
            new WaitAction(this, 0.5f),
            new ShowTextAction(subtitleManager, "This is a test dialogue!"),
            new TriggerEventAction(() => Debug.Log("Custom event triggered!"))
        };

        PlayDialogue(actions);
    }
}
