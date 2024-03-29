﻿using System;
using Orbitality;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "OrbitalitySettings", menuName = "ScriptableObjects/OrbitalitySettings")]
    public class OrbitalitySettings : ScriptableObject, IPlanetGenerationSettings
    {
        public int MaxPlanets;

        public float MinOrbitDistanceX;
        public float MaxOrbitDistanceX;
        public float OrbitScaleY;

        public float MinAngularDisplacement;
        public float MaxAngularDisplacement;

        public float MaxRadius;
        public float MinRadius;

        float IPlanetGenerationSettings.MaxOrbitDistanceX => MaxOrbitDistanceX;
        float IPlanetGenerationSettings.MinRadius => MinRadius;
        float IPlanetGenerationSettings.MinOrbitDistanceX => MinOrbitDistanceX;
        float IPlanetGenerationSettings.MaxRadius => MaxRadius;
        float IPlanetGenerationSettings.OrbitScaleY => OrbitScaleY;

        float IPlanetGenerationSettings.MinAngularDisplacement => MinAngularDisplacement;
        float IPlanetGenerationSettings.MaxAngularDisplacement => MaxAngularDisplacement;
    }

    public interface IPlanetGenerationSettings
    {
        float MaxOrbitDistanceX { get; }
        float MinRadius { get; }
        float MinOrbitDistanceX { get; }
        float MaxRadius { get; }
        float OrbitScaleY { get; }
        float MinAngularDisplacement { get; }
        float MaxAngularDisplacement { get;  }
    }
    [Serializable]
    public class OrbitalityMissileSkin
    {
        public Sprite Sprite;
    }

    [Serializable]
    public class OrbitalityPlanetSkin
    {
        public Sprite Sprite;
    }
}