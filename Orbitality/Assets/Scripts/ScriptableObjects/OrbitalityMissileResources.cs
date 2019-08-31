using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "OrbitalityMissileResources", menuName = "ScriptableObjects/OrbitalityMissileResources")]

    public class OrbitalityMissileResources:ScriptableObject
    {
        public OrbitalityMissileSkin[] Skins;
    }
}