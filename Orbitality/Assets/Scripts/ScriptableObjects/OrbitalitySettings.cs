using System.Collections;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "OrbitalitySettings", menuName = "ScriptableObjects/OrbitalitySettings")]
    public class OrbitalitySettings : ScriptableObject
    {
        [SerializeField] public Sprite[] PlanetSprites;
        [SerializeField] public GameObject[] Missiles;

        public float OrbitDistance;
    }
}