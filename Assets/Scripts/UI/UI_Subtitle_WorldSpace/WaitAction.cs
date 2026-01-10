using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Action that waits for a specified duration before completing
/// </summary>
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
