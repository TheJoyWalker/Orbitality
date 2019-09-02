using Orbitality;

namespace Orbitality.Saves
{
    public class PlanetSaveBuilder
    {
        public PlanetSaveData Build(Planet planet)
        {
            PlanetSaveData data = new PlanetSaveData()
                                  {
                                      PlanetData = planet.PlanetData,
                                      Health = planet.Health,
                                      MaxHealth = planet.MaxHealth,
                                  };

            return data;
        }
    }
}