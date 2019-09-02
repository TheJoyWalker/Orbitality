using Orbitality;
using UnityEngine;
// ReSharper disable All
#pragma warning disable 649

namespace UI.Menu
{

    public class LoadMenu : MonoBehaviour
    {
        [SerializeField] private OrbitalityGame Prefab;
        [SerializeField] private LoadLevelButton _levelButtonPrefab;
        [SerializeField] private Transform LevelList;

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
                Debug.Log($"got a save{save}");
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
            Debug.LogError("start loading here!");
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

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}