using ScriptableObjects;
using UnityEngine;

namespace Orbitality
{
    public class PlanetMotionResolver
    {
        public Vector2 Resolve(PlanetData planetData, float time, IPlanetGenerationSettings settings)
        {
            return new Vector2(planetData.OrbitRadius * Mathf.Sin(time * planetData.AngularDisplacement),
                               planetData.OrbitRadius * Mathf.Cos(time * planetData.AngularDisplacement) *
                               settings.OrbitScaleY);
        }
    }
}