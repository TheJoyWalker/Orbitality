using System;
using JetBrains.Annotations;
using UnityEngine;

public class PauseManager : MonoStaticUtility<PauseManager>
{
    [RuntimeInitializeOnLoadMethod]
    public static void Initialize() => InitUnityObject(true);

    private static bool _paused;

    private static bool _appPaused;

    private static bool _isFocused = true;

    private static bool _computedPaused;

    /// <summary>
    ///     User pause requests
    /// </summary>
    public static bool Paused
    {
        get => _paused;
        set
        {
            _paused = value;
            UpdatePauseChanged();
        }
    }

    /// <summary>
    ///     Application pause
    /// </summary>
    public static bool AppPaused
    {
        get => _appPaused;
        private set
        {
            _appPaused = value;
            UpdatePauseChanged();
        }
    }

    /// <summary>
    ///     Application focus
    /// </summary>
    public static bool IsFocused
    {
        get => _isFocused;
        private set
        {
            _isFocused = value;
            UpdatePauseChanged();
        }
    }

    public static bool ComputedPaused
    {
        get => _computedPaused;
        set
        {
            _computedPaused = value;
            OnPauseChanged(value);
        }
    }

    public static event EventHandler<bool> PauseChanged;

    private static void UpdatePauseChanged() => ComputedPaused = !_paused && _isFocused && !_appPaused;


    [UsedImplicitly]
    private void OnApplicationPause(bool isPaused) => AppPaused = isPaused;

    [UsedImplicitly]
    private void OnApplicationFocus(bool hasFocus) => _isFocused = hasFocus;

    private static void OnPauseChanged(bool e) => PauseChanged?.Invoke(null, e);
}