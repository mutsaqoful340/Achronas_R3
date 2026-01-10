using System;
using UnityEngine;

/// <summary>
/// Action that displays text using PerLetterSubtitleManager
/// </summary>
public class ShowTextAction : IDialogueAction
{
    private PerLetterSubtitleManager subtitleManager;
    private string text;

    public ShowTextAction(PerLetterSubtitleManager manager, string text)
    {
        this.subtitleManager = manager;
        this.text = text;
    }

    public void Execute(Action onComplete)
    {
        if (subtitleManager == null)
        {
            Debug.LogError("ShowTextAction: SubtitleManager is null!");
            onComplete?.Invoke();
            return;
        }

        subtitleManager.StartSubtitle(text, onComplete);
    }
}
