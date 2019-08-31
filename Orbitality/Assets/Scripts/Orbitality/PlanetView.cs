using ScriptableObjects;
using UnityEngine;

namespace Orbitality
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class PlanetView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
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




        void Update()
        {
#if UNITY_EDITOR
            UpdateView();
#endif
        }

        private void UpdateView()
        {
            _spriteRenderer.sprite = Resources.Skins[SkinId].Sprite;
            transform.localScale = Vector3.one * Radius * 0.5f;
        }
    }
}