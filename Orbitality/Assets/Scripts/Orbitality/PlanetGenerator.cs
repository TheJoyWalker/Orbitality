using ScriptableObjects;

namespace Orbitality
{
    public class PlanetGenerator
    {
        public PlanetData[] Generate(int count, IPlanetGenerationSettings settings)
        {
            var planets = new PlanetData[count];
            var orbitRadiusVariance = settings.MaxOrbitDistanceX - settings.MinOrbitDistanceX;
            var radiusVariance = settings.MaxRadius - settings.MinRadius;
            var stepFactor = 1f / planets.Length;
            for (var i = 0; i < planets.Length; i++)
            {
                var item = planets[i] = new PlanetData();
                var step = i + 1;
                item.OrbitRadius = settings.MinOrbitDistanceX + orbitRadiusVariance * stepFactor * step;
                item.Radius = settings.MinRadius + radiusVariance * stepFactor * step;
                item.OrbitScaleX = settings.OrbitScaleY;
                item.AngularDisplacement = stepFactor * step;
            }

            return planets;
        }
    }
}