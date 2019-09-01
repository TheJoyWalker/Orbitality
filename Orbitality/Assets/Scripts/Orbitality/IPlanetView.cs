using ScriptableObjects;
using UnityEngine;

namespace Orbitality
{
    public interface IPlanetView
    {
        Vector3 BarPosition { get; }
        Vector3 Position { get; set; }
        float Radius { get; set; }
        OrbitalityPlanetResources Resources { get; set; }
        int SkinId { get; set; }
        Planet Owner { get; set; }
    }
}