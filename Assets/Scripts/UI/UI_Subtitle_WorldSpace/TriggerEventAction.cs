using System;
using UnityEngine;

/// <summary>
/// Action that triggers a UnityEvent (for inspector wiring)
/// </summary>
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
