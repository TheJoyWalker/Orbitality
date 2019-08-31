using ScriptableObjects;
using UnityEngine;

namespace Orbitality
{
    public class PlanetView : MonoBehaviour, IPlanetView
    {
        public Vector3 Position
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
        [SerializeField] private SpriteRenderer _spriteRenderer = default;
        [SerializeField] private OrbitalityPlanetResources _resources;

        public OrbitalityPlanetResources Resources
        {
            get { return _resources; }
            set { _resources = value; }
        }


        [SerializeField] private int _skinId;

        public int SkinId
        {
            get { return _skinId; }
            set { _skinId = value; }
        }

        [SerializeField] private float _radius;

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        private void UpdateView()
        {
            _spriteRenderer.sprite = Resources.Skins[SkinId].Sprite;
            transform.localScale = Vector3.one * Radius * 0.5f;
        }
    }
}