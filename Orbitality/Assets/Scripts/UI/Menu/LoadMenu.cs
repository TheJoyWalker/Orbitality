using Orbitality;
using UnityEngine;
// ReSharper disable All
#pragma warning disable 649

namespace UI.Menu
{

    public class LoadMenu : MonoBehaviour
    {
        [SerializeField] private OrbitalityGame Game;
        [SerializeField] private LoadLevelButton _levelButtonPrefab;
        [SerializeField] private Transform LevelList;
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private RectTransform _menuBackground;


        //todo: think of a better binding
        private SaveManager _saveManager;

        void Awake()
        {
            _saveManager = new SaveManager();
            Show();
            LoadItems();
        }
        public void LoadItems()
        {
            Clear();
            foreach (var save in _saveManager.GetSaves())
            {
                var displayName = _saveManager.GetDisplayName(save);

                var button = Instantiate(_levelButtonPrefab, LevelList);
                button.SetData(save, displayName);
                button.Clicked += ButtonOnClicked;
                button.gameObject.SetActive(true);
            }
        }

        private void ButtonOnClicked(LoadLevelButton sender) => LoadLevel(sender.FileName);

        private void LoadLevel(string objFileName)
        {
            Game.Load(_saveManager.Load(objFileName));
            Hide();
            _menuBackground.gameObject.SetActive(false);
            PauseManager.UserPaused = false;

        }

        private void Clear()
        {
            for (int i = 0; i < LevelList.childCount; i++)
            {
                var t = LevelList.GetChild(i);
                if (_levelButtonPrefab.transform == t)
                    continue;

                Destroy(t.gameObject);
                i--;
            }
        }

        public void OnCancel()
        {
            Hide();
            _mainMenu.Show();
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}