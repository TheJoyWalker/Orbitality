using Orbitality;
using UnityEngine;
#pragma warning disable 649

namespace UI.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private OrbitalityGame _game;
        [SerializeField] private LoadMenu _loadMenu;
        [SerializeField] private RectTransform _menuBackground;

        public void OnStart()
        {
            _game.StartGame();
            Hide();
            _menuBackground.gameObject.SetActive(false);
            PauseManager.UserPaused = false;
        }

        public void OnLoad()
        {
            Hide();
            _loadMenu.Show();
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}