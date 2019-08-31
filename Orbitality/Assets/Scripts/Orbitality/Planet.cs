using UnityEngine;

namespace Orbitality
{
    [SerializeField]
    public class Planet
    {
        public Planet(PlanetData data, PlanetView view)
        {
            PlanetData = data;
            View = view;
        }

        [SerializeField] public PlanetData PlanetData { get; }

        public PlanetView View { get; }
        public int Health { get; private set; }
        public float Cooldown { get; private set; }

        public void Fire()
        {
        }
    }
}