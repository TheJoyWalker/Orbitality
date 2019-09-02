using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbitality.Saves
{
    public class OrbitalitySaveBuilder
    {
        private readonly PlanetSaveBuilder _planetBuilder = new PlanetSaveBuilder();
        private readonly MissileSaveBuilder _missileBuilder = new MissileSaveBuilder();
        public OrbitalitySave Build(IList<Planet> planets, Missile[] missiles, float TimeDone, int playerIdx)
        {
            if (playerIdx == -1)
                throw new ArgumentException("playerIdx can not be -1");
            if (planets.Count <= 1)
                throw new ArgumentException("planets count should be > 2");

            OrbitalitySave save = new OrbitalitySave()
            {
                Planets = planets.Select(x => _planetBuilder.Build(x)).ToArray(),
                Missiles = missiles.Select(x => _missileBuilder.Build(planets, x)).ToArray(),
                Time = TimeDone,
                PlayerIdx = playerIdx,
            };
            save.Version = "0.1";
            return save;
        }
    }
}
