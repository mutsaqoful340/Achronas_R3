using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Complete dialogue system with action list pattern.
/// Manages sequential dialogue execution with callbacks.
/// Contains all action types as nested classes.
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SubtitleSystem subtitleSystem;
    [SerializeField] private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool logActionExecution = true;

    private List<IDialogueAction> currentActionList;
    private int currentActionIndex = 0;
    private bool isPlayingDialogue = false;

    public bool IsPlaying => isPlayingDialogue;

    // ===== PUBLIC API =====

    public void PlayDialogue(List<IDialogueAction> actions)
    {
        if (isPlayingDialogue)
        {
            Debug.LogWarning("DialogueSystem: Already playing dialogue!");
            return;
        }

        if (actions == null || actions.Count == 0)
        {
            Debug.LogWarning("DialogueSystem: No actions to execute!");
            return;
        }

        currentActionList = actions;
        currentActionIndex = 0;
        isPlayingDialogue = true;

        if (logActionExecution)
            Debug.Log($"DialogueSystem: Starting dialogue with {actions.Count} actions");

        ExecuteNextAction();
    }

    public void PlayDialogue(DialogueSequenceSO dialogueSequence)
    {
        if (dialogueSequence == null)
        {
            Debug.LogError("DialogueSystem: DialogueSequence is null!");
            return;
        }

        if (dialogueSequence.lines == null || dialogueSequence.lines.Count == 0)
        {
            Debug.LogWarning("DialogueSystem: DialogueSequence has no lines!");
            return;
        }

        var actions = new List<IDialogueAction>();

        if (dialogueSequence.startSound != null && audioSource != null)
        {
            actions.Add(new PlaySoundAction(audioSource, dialogueSequence.startSound, false));
        }

        foreach (var line in dialogueSequence.lines)
        {
            if (!string.IsNullOrEmpty(line.text))
            {
                actions.Add(new ShowTextAction(subtitleSystem, line.text));

                if (line.delayAfter > 0)
                {
                    actions.Add(new WaitAction(this, line.delayAfter));
                }
            }
        }

        if (dialogueSequence.endSound != null && audioSource != null)
        {
            actions.Add(new PlaySoundAction(audioSource, dialogueSequence.endSound, false));
        }

        PlayDialogue(actions);
    }

    public void StopDialogue()
    {
        if (logActionExecution)
            Debug.Log("DialogueSystem: Dialogue stopped");

        isPlayingDialogue = false;
        currentActionList = null;
        currentActionIndex = 0;
    }

    // ===== INTERNAL LOGIC =====

    private void ExecuteNextAction()
    {
        if (currentActionIndex >= currentActionList.Count)
        {
            OnDialogueComplete();
            return;
        }

        IDialogueAction action = currentActionList[currentActionIndex];

        if (logActionExecution)
            Debug.Log($"DialogueSystem: Executing action {currentActionIndex + 1}/{currentActionList.Count} ({action.GetType().Name})");

        currentActionIndex++;
        action.Execute(() => OnActionComplete());
    }

    private void OnActionComplete()
    {
        if (!isPlayingDialogue) return;
        ExecuteNextAction();
    }

    private void OnDialogueComplete()
    {
        if (logActionExecution)
            Debug.Log("DialogueSystem: Dialogue sequence complete");

        isPlayingDialogue = false;
        currentActionList = null;
        currentActionIndex = 0;
    }

    [ContextMenu("Test Dialogue Sequence")]
    public void TestDialogueSequence()
    {
        var actions = new List<IDialogueAction>
        {
            new ShowTextAction(subtitleSystem, "Hello there!"),
            new WaitAction(this, 1.0f),
            new ShowTextAction(subtitleSystem, "How are you today?"),
            new WaitAction(this, 0.5f),
            new ShowTextAction(subtitleSystem, "This is a test dialogue!"),
            new TriggerEventAction(() => Debug.Log("Custom event triggered!"))
        };

        PlayDialogue(actions);
    }

    // ===== NESTED INTERFACE & CLASSES =====

    public interface IDialogueAction
    {
        void Execute(Action onComplete);
    }

    public class ShowTextAction : IDialogueAction
    {
        private SubtitleSystem subtitleSystem;
        private string text;

        public ShowTextAction(SubtitleSystem system, string text)
        {
            this.subtitleSystem = system;
            this.text = text;
        }

        public void Execute(Action onComplete)
        {
            if (subtitleSystem == null)
            {
                Debug.LogError("ShowTextAction: SubtitleSystem is null!");
                onComplete?.Invoke();
                return;
            }

            subtitleSystem.StartSubtitle(text, onComplete);
        }
    }

    public class WaitAction : IDialogueAction
    {
        private MonoBehaviour coroutineRunner;
        private float duration;

        public WaitAction(MonoBehaviour runner, float seconds)
        {
            this.coroutineRunner = runner;
            this.duration = seconds;
        }

        public void Execute(Action onComplete)
        {
            coroutineRunner.StartCoroutine(WaitCoroutine(onComplete));
        }

        private IEnumerator WaitCoroutine(Action onComplete)
        {
            yield return new WaitForSeconds(duration);
            onComplete?.Invoke();
        }
    }

    public class PlaySoundAction : IDialogueAction
    {
        private AudioSource audioSource;
        private AudioClip clip;
        private bool waitForCompletion;

        public PlaySoundAction(AudioSource source, AudioClip clip, bool waitForCompletion = false)
        {
            this.audioSource = source;
            this.clip = clip;
            this.waitForCompletion = waitForCompletion;
        }

        public void Execute(Action onComplete)
        {
            if (audioSource == null || clip == null)
            {
                Debug.LogWarning("PlaySoundAction: AudioSource or Clip is null!");
                onComplete?.Invoke();
                return;
            }

            audioSource.PlayOneShot(clip);

            if (waitForCompletion)
            {
                audioSource.GetComponent<MonoBehaviour>()?.StartCoroutine(
                    WaitForSound(clip.length, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        private IEnumerator WaitForSound(float duration, Action onComplete)
        {
            yield return new WaitForSeconds(duration);
            onComplete?.Invoke();
        }
    }

    public class TriggerEventAction : IDialogueAction
    {
        private Action customAction;

        public TriggerEventAction(Action action)
        {
            this.customAction = action;
        }

        public void Execute(Action onComplete)
        {
            customAction?.Invoke();
            onComplete?.Invoke();
        }
    }
}
