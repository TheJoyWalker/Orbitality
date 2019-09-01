using Orbitality;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "OrbitalityPlanetResources", menuName = "ScriptableObjects/OrbitalityPlanetResources")]
    public class OrbitalityPlanetResources : ScriptableObject
    {
        public PlanetView Prefab;
        public ParticleExplosionController ExplosionPrefab;
        public OrbitalityPlanetSkin[] Skins;
    }
}