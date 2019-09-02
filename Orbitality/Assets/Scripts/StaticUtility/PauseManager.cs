using System;
using JetBrains.Annotations;
using UnityEngine;

public class PauseManager : MonoStaticUtility<PauseManager>
{
    [RuntimeInitializeOnLoadMethod]
    public static void Initialize() => InitUnityObject(true);

    private static bool _userPaused;

    private static bool _appPaused;

    private static bool _isFocused = true;

    private static bool _paused;

    /// <summary>
    ///     User pause requests
    /// </summary>
    public static bool UserPaused
    {
        get => _userPaused;
        set
        {
            _userPaused = value;
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

    public static bool Paused
    {
        get => _paused;
        set
        {
            var saveValue = _paused == value;
            _paused = value;
            if (!saveValue)
                OnPauseChanged(value);
        }
    }

    public static event EventHandler<bool> PauseChanged;

    private static void UpdatePauseChanged() => Paused = UserPaused || AppPaused || !IsFocused;


    [UsedImplicitly]
    private void OnApplicationPause(bool isPaused) => AppPaused = isPaused;

    [UsedImplicitly]
    private void OnApplicationFocus(bool hasFocus) => _isFocused = hasFocus;

    private static void OnPauseChanged(bool e) => PauseChanged?.Invoke(null, e);
}