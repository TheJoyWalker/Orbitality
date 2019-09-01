using System;
using System.Collections;
using UnityEngine;

public class CountdownNotifier : MonoBehaviour
{
    //todo: declare proper event delegates
    public event Action<CountdownNotifier> Completed;
    private Coroutine _countDownCoroutine;
    public void StartCountdown(float time)
    {
        if (_countDownCoroutine != null)
            StopCoroutine(_countDownCoroutine);
        _countDownCoroutine = StartCoroutine(CountdownCoroutine(time));
    }

    private IEnumerator CountdownCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
    }

    protected virtual void OnCompleted(CountdownNotifier obj) => Completed?.Invoke(obj);
}