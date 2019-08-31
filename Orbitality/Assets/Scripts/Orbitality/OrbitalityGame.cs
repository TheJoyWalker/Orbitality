using System;
using System.Collections.Generic;
using System.Linq;
using Pools;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Orbitality
{
    public class OrbitalityGame : MonoBehaviour
    {
        #region Utility
        private readonly PlanetMotionResolver _motionResolver = new PlanetMotionResolver();
        private readonly PlanetGenerator _planetGenerator = new PlanetGenerator();
        //private readonly AimManager _aimManager;
        #region Pools
        private SimpleMonoPool<PlanetView> _planetPool;
        #endregion
        #endregion

        #region ScriptableObjects
        [SerializeField] private OrbitalitySettings _settings = default;
        [SerializeField] private OrbitalityPlanetResources _planetResources = default;
        [SerializeField] private OrbitalityMissileResources _missileResources = default;
        #endregion
        private List<Planet> _planets = new List<Planet>();
        [SerializeField] private float TimeDone;

        private void Awake()
        {
            _planetPool = new SimpleMonoPool<PlanetView>(_planetResources.Prefab);
            _planets = _planetGenerator.Generate(_settings.MaxPlanets, _settings)
                                       .Select(x => new Planet(x, _planetPool.Spawn(v =>
                                                                                    {
                                                                                        v.Resources = _planetResources;
                                                                                        v.Radius = x.Radius;
                                                                                        v.SkinId = GetFreeSkinId();
                                                                                    }))).ToList();
            Debug.Log($"Create {_planets.Count} planets.");
        }

        private void FixedUpdate()
        {
            foreach (var planet in _planets)
                planet.View.transform.localPosition =
                    _motionResolver.Resolve(planet.PlanetData, TimeDone, _settings);

            TimeDone += Time.fixedDeltaTime;
        }

        private int GetFreeSkinId()
        {
            //todo: consider this later
            var freeSkins = _planetResources.Skins.Where((skin, idx) =>
                                                         {
                                                             return _planets.Any(data => data.PlanetData.SkinId == idx);
                                                         }).ToList();
            return Random.Range(0, freeSkins.Count);
        }
    }


    [Serializable]
    public class PlanetData
    {
        public float AngularDisplacement;
        public float Cooldown;
        public float OrbitRadius;
        public float OrbitScaleX;
        public float Radius;
        public int SkinId;
    }
}