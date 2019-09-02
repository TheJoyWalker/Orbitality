using System;

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace UI.Menu
{
    public class LoadLevelButton : MonoBehaviour
    {
        [SerializeField] private Text _label;
        public event Action<LoadLevelButton> Clicked;
        private string _fileName;
        public string FileName => _fileName;

        public void SetData(string filename, string displayName)
        {
            _fileName = filename;
            _label.text = displayName;
        }

        [UsedImplicitly]
        public void FireClick() => OnClicked(this);
        protected virtual void OnClicked(LoadLevelButton obj) => Clicked?.Invoke(obj);
    }
}
