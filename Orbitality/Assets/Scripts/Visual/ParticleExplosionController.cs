using System;
using UnityEngine;
[RequireComponent(typeof(CountdownNotifier))]
public class ParticleExplosionController : MonoBehaviour
{
    [SerializeField] private CountdownNotifier _countdownNotifier;
    [SerializeField] private ParticleSystem _particleSystem;
    //todo: declare proper event delegates
    public event Action<ParticleExplosionController> Completed;

    void OnEnable() => _countdownNotifier.Completed += CountdownNotifierOnCompleted;
    void OnDisable() => _countdownNotifier.Completed -= CountdownNotifierOnCompleted;

    private void CountdownNotifierOnCompleted(CountdownNotifier obj) => OnCompleted(this);

    public void Explode()
    {
        _particleSystem.Play(true);
        _countdownNotifier.StartCountdown(_particleSystem.main.duration);
    }

    void Reset()
    {
        _countdownNotifier = GetComponent<CountdownNotifier>();
        _particleSystem = GetComponent<ParticleSystem>();
    }
    protected virtual void OnCompleted(ParticleExplosionController obj) => Completed?.Invoke(obj);
}