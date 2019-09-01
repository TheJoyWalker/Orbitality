using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Orbitality;
using Assets.Scripts.UI;
using JetBrains.Annotations;
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
        private OrbitalityUserInput _orbitalityUserInput;
        private WorldToLocalTransformer _transformer;
        private FireController _fireController;
        private OrbitalityAI _ai;
        #region Pools
        private SimpleMonoPool<PlanetView> _planetViewPool;
        private Dictionary<int, SimpleMonoPool<Missile>> _missilePools = new Dictionary<int, SimpleMonoPool<Missile>>();
        private SimpleMonoPool<ParticleExplosionController> _planetExplosionPool;
        #endregion
        #endregion

        #region ScriptableObjects
        [SerializeField] private OrbitalityMonoSettings _settings = default;
        [SerializeField] private OrbitalityPlanetResources _planetResources = default;
        [SerializeField] private OrbitalityMissileResources _missileResources = default;
        #endregion

        [SerializeField] private PlanetBarManager _planetBarManager = default;
        [SerializeField] private Collider _pointerInputCollider = default;
        [SerializeField] private float _timeDone;

        public event Action<OrbitalityGame> Win;
        public event Action<OrbitalityGame> Loose;

        private List<Planet> _planets = new List<Planet>();
        private SimpleMonoPool<ParticleExplosionController> _missileExplosionPool;

        private void Awake()
        {
            Initialize();
            GeneratePlanets();
            SetUpPlayer();
            Debug.Log($"Create {_planets.Count} planets.");
        }

        private void Initialize()
        {
            _transformer = new WorldToLocalTransformer(transform);
            _planetViewPool = new SimpleMonoPool<PlanetView>(_planetResources.Prefab);
            _planetExplosionPool = new SimpleMonoPool<ParticleExplosionController>(_planetResources.ExplosionPrefab);
            _missileExplosionPool = new SimpleMonoPool<ParticleExplosionController>(_missileResources.ExplosionPrefab);

            _ai = new OrbitalityAI();
            for (var i = 0; i < _missileResources.Missiles.Length; i++)
            {
                _missilePools[i] = new SimpleMonoPool<Missile>(_missileResources.Missiles[i]);
            }
            _fireController = new FireController(_missilePools, _missileExplosionPool, transform);
        }

        private void GeneratePlanets()
        {
            var skinIdxsTemp = Enumerable.Range(0, _planetResources.Skins.Length - 1).ToList();
            var planetCount = _settings.MaxPlanets;
            var skinIdxs = new int[planetCount];

            for (int i = 0; i < planetCount; i++)
            {
                int idx = Random.Range(0, skinIdxsTemp.Count);
                skinIdxs[i] = skinIdxsTemp[idx];
                skinIdxsTemp.RemoveAt(idx);
            }

            _planets = _planetGenerator.Generate(planetCount, _settings)
                                       .Select((x, idx) =>
                                                   new Planet(x, _planetViewPool.Spawn(v => PreparePlanetView(x, v, skinIdxs[idx])))).ToList();
            foreach (var planet in _planets)
            {
                planet.Died += PlanetOnDied;
            }

        }

        private void PreparePlanetView(PlanetData planet, PlanetView view, int skinIdx)
        {
            planet.Cooldown = 3f;//TODO: create weaponry info stuff
            view.transform.parent = transform;
            view.Resources = _planetResources;
            view.Radius = planet.Radius;
            view.SkinId = skinIdx;
        }

        private void SetUpPlayer()
        {
            var playerIdx = Random.Range(0, _planets.Count);
            _planets[playerIdx].IsPlayerControlled = true;
            _orbitalityUserInput = new OrbitalityUserInput(_fireController, _transformer, _planets[playerIdx]);
        }

        private void PlanetOnDied(HealthAgentDeathArgs args)
        {
            Debug.Log($"PlanetOnDied: {args.Victim}");
            //todo:get rid of view here
            var planet = (Planet)args.Victim;
            var explosion = _planetExplosionPool.Spawn(x =>
                                                       {
                                                           x.transform.SetParent(transform);
                                                           x.transform.position = planet.Position;
                                                       });
            Debug.Log($"Explode: {explosion.name}");
            explosion.Explode();
            explosion.Completed += ExplosionOnCompleted;
            _planetViewPool.Release((PlanetView)planet.View);
            _planets.Remove(planet);
            _planetBarManager.Remove(planet);

            CheckWinLoose(planet);
        }

        private void CheckWinLoose(Planet deceased)
        {
            if (deceased.IsPlayerControlled)
            {
                Debug.LogError("loose");
                OnLoose(); 
            }

            if (_planets.Count == 1 && _planets[0].IsPlayerControlled)
            {
                Debug.LogError("win");
                OnWin();
            }
        }

        //todo:replace with proper handlers
        protected virtual void OnWin() => Win?.Invoke(this);
        protected virtual void OnLoose() => Loose?.Invoke(this);
        private void ExplosionOnCompleted(ParticleExplosionController obj) => _planetExplosionPool.Release(obj);

        #region Monobehaviour methods
        [UsedImplicitly]
        private void FixedUpdate()
        {
            foreach (var planet in _planets)
                planet.Position = _motionResolver.Resolve(planet.PlanetData, _timeDone, _settings);
            _ai.Tick(_planets, _fireController);
            _timeDone += Time.fixedDeltaTime;
        }
        [UsedImplicitly]
        private void OnEnable()
        {
            PointerHitResolver.Subscribe(_pointerInputCollider, _orbitalityUserInput);
            _planetBarManager.AddRange(_planets);
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            PointerHitResolver.Unsubscribe(_pointerInputCollider, _orbitalityUserInput);
            _planetBarManager.RemoveRange(_planets);
        }
        #endregion


    }
}