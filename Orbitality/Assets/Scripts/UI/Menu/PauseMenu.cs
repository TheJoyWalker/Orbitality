using Orbitality;
using UnityEngine;
#pragma warning disable 649

namespace UI.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private OrbitalityGame _orbitalityGame;
        //todo: think of a better binding
        private SaveManager _saveManager;
        public void Awake() => _saveManager=new SaveManager();
        public void Save() => _saveManager.Save(_orbitalityGame.GetSave());
        public void Load()
        {
            Debug.Log("try loading last");
            var orbitalitySave = _saveManager.LoadLast();
            Debug.Log("game loading save");
            _orbitalityGame.Load(orbitalitySave);
        }
    }
}