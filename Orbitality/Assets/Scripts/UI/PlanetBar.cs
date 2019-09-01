using Orbitality;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace Assets.Scripts.UI
{
    public class PlanetBar : MonoBehaviour
    { 
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _cooldownText;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _foreground;
        [SerializeField] private Image _cooldownImage;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private Color highHealthColor = Color.green;
        public Canvas _canvas;

        //TODO: protect the property and use events
        public Planet Planet;
        void Update()
        {
            _slider.value = (float)Planet.Health / Planet.MaxHealth;
            _foreground.color = Color.Lerp(lowHealthColor, highHealthColor, _slider.value);
            _cooldownText.text = $"{Planet.Cooldown:F2}s";
            _rectTransform.anchoredPosition = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, Planet.BarPosition);
            _cooldownImage.fillAmount = Planet.ChargePercent;
        }
    }
}