using UnityEngine;

namespace Orbitality
{
    [SerializeField]
    public class Planet
    {
        public Vector3 Position
        {
            get => View.Position;
            set => View.Position = value;
        }
        public Planet(PlanetData data, IPlanetView view)
        {
            PlanetData = data;
            View = view;
        }

        [SerializeField] public PlanetData PlanetData { get; }
        public IPlanetView View { get; }
        public int Health { get; private set; }
        public float Cooldown { get; private set; }

        public void Fire()
        {
        }
    }
}