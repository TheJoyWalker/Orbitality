using Orbitality;
using UnityEditor;
using UnityEngine;
#pragma warning disable 649

namespace UI.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform _menuBackground;
        [SerializeField] private OrbitalityGame _orbitalityGame;
        [SerializeField] private LoadMenu _loadMenu;
        //todo: think of a better binding
        private SaveManager _saveManager;
        public void Awake()
        {
            _saveManager = new SaveManager();
            PauseManager.PauseChanged += PauseManagerOnPauseChanged;
        }

        private void PauseManagerOnPauseChanged(object sender, bool e)
        {
            if (!PauseManager.UserPaused)
                PauseManager.UserPaused = PauseManager.UserPaused;
            //ensure pause on focus loss

            if (!PauseManager.Paused)
            {
                _menuBackground.gameObject.SetActive(false);
                foreach (Transform menu in _menuBackground)
                {
                    menu.gameObject.SetActive(false);
                }
            }
        }

        public void StartNew()
        {
            _orbitalityGame.StartGame();
            HideWithBack();
        }

        private void HideWithBack()
        {
            PauseManager.UserPaused = false;
            Hide();
            _menuBackground.gameObject.SetActive(false);
        }

        public void Save()
        {
            _saveManager.Save(_orbitalityGame.GetSave());
            HideWithBack();
        }

        public void Load()
        {
            Hide();
            _loadMenu.Show();
        }


        public void Show()
        {
            _menuBackground.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}