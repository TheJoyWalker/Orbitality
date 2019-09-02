using Assets.Scripts.UI.Menu;
using Orbitality;
using UI.Menu;
using UnityEngine;
#pragma warning disable 649

/// <summary>
/// should add firing
/// </summary>
public class OrbitalityTempPauseWinHandler : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private WinLooseMenu _winLooseMenu;
    [SerializeField] private OrbitalityGame _game;
    void Awake()
    {
        PauseManager.PauseChanged += PauseManagerOnPauseChanged;
        _winLooseMenu.Game = _game;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseManager.UserPaused = !PauseManager.UserPaused;
            if (PauseManager.Paused)
            {
                _pauseMenu.Show();
            }
        }
    }

    private void PauseManagerOnPauseChanged(object sender, bool e)
    {
        PauseManager.PauseChanged -= PauseManagerOnPauseChanged;
    }
}
