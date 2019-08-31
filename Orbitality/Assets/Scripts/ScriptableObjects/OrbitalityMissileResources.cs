using Orbitality;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "OrbitalityMissileResources", menuName = "ScriptableObjects/OrbitalityMissileResources")]

    public class OrbitalityMissileResources:ScriptableObject
    {
        public Missile[] Missiles;
        public OrbitalityMissileSkin[] Skins;
    }
}