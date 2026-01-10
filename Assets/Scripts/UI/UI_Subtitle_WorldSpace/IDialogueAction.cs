using System;

/// <summary>
/// Base interface for all dialogue actions.
/// Each action executes and calls onComplete when finished.
/// </summary>
public interface IDialogueAction
{
    void Execute(Action onComplete);
}
