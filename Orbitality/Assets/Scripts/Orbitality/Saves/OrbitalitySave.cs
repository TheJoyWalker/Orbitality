using System;

namespace Orbitality.Saves
{
    [Serializable]
    public class OrbitalitySave
    {
        public string Version = "0.1";
        public PlanetSaveData[] Planets;
        public MissileSaveData[] Missiles;
        public int PlayerIdx;
        public float Time;
    }
}