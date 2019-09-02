using ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Orbitality
{
    public class OrbitalityMonoSettings : MonoBehaviour, IPlanetGenerationSettings
    {
        public int MaxPlanets;

        public float MinOrbitDistanceX;
        public float MaxOrbitDistanceX;
        public float OrbitScaleY;

        public float MinAngularDisplacement;
        public float MaxAngularDisplacement;

        public float MinRadius;
        public float MaxRadius;

        float IPlanetGenerationSettings.MaxOrbitDistanceX => MaxOrbitDistanceX;
        float IPlanetGenerationSettings.MinRadius => MinRadius;
        float IPlanetGenerationSettings.MinOrbitDistanceX => MinOrbitDistanceX;
        float IPlanetGenerationSettings.MaxRadius => MaxRadius;
        float IPlanetGenerationSettings.OrbitScaleY => OrbitScaleY;

        float IPlanetGenerationSettings.MinAngularDisplacement => MinAngularDisplacement;
        float IPlanetGenerationSettings.MaxAngularDisplacement => MaxAngularDisplacement;
    }
}