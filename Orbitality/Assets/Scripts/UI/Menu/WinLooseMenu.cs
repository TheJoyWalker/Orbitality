using Orbitality;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Menu
{
    public class WinLooseMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform _menuBackground;
        [SerializeField] private Text _text;
        private OrbitalityGame _game;

        public OrbitalityGame Game
        {
            get { return _game; }
            set
            {
                _game = value;
                _game.Win += GameOnWin;
                _game.Loose += GameOnLoose;
            }
        }


        private void GameOnWin(OrbitalityGame obj)
        {
            Show();
            _text.text = "You loose!";
        }

        private void GameOnLoose(OrbitalityGame obj)
        {
            Show();
            _text.text = "You win!";
        }


        private void Prepare()
        {
            _game.gameObject.SetActive(false);
            _menuBackground.gameObject.SetActive(true);
            foreach (Transform menu in _menuBackground)
            {
                menu.gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            Prepare();
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        public void OnRestartButton()
        {
            _game.gameObject.SetActive(false);
            Hide();
            _game.StartGame();
        }
    }
}