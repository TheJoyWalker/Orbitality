using System;
using Orbitality;

namespace Orbitality.Saves
{
    [Serializable]
    public class PlanetSaveData
    {
        public PlanetData PlanetData;
        public int MaxHealth;
        public int Health;
    }
}