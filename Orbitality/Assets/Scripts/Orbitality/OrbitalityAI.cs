using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Orbitality
{
    public class OrbitalityAI
    {
        //todo: optimize to reduce GC
        public void Tick(List<Planet> planets, FireController fireController)
        {
            switch (planets.Count)
            {
                case 0:
                case 1 when planets[0].IsPlayerControlled:
                    return;
            }

            foreach (var planet in planets.Where(x => x.CanShoot && !x.IsPlayerControlled))
            {
                fireController.Fire(planet.PlanetData.WeaponType, planet, FindAim(planet, planets).Position);
                planet.RegisterShot();
            }
        }

        private Planet FindAim(Planet planet, List<Planet> planets)
        {
            //todo: optimize target selection
            return planets.Except(new[] { planet }).OrderBy(x => Vector2.Distance(x.Position, planet.Position))
                          .FirstOrDefault();
        }
    }
}