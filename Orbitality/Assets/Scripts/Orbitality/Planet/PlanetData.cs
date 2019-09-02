using System;

namespace Orbitality
{
    [Serializable]
    public class PlanetData
    {
        public float AngularDisplacement;
        public float Cooldown;
        public float OrbitRadius;
        public float OrbitScaleX;
        public float Radius;
        public int SkinId;
        public int WeaponType;

        public int MaxHealth = 100;
    }
}