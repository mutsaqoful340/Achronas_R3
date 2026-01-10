using System;
using UnityEngine;

/// <summary>
/// Action that plays an AudioClip
/// </summary>
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
            // Wait for clip to finish
            audioSource.GetComponent<MonoBehaviour>()?.StartCoroutine(
                WaitForSound(clip.length, onComplete));
        }
        else
        {
            // Complete immediately (fire and forget)
            onComplete?.Invoke();
        }
    }

    private System.Collections.IEnumerator WaitForSound(float duration, Action onComplete)
    {
        yield return new UnityEngine.WaitForSeconds(duration);
        onComplete?.Invoke();
    }
}
