﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Orbitality;
using UI;
using JetBrains.Annotations;
using Orbitality.Saves;
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
        private OrbitalityUserMissileInput _orbitalityUserMissileInput;
        private WorldToLocalTransformer _transformer;
        private FireController _fireController;
        private OrbitalityAI _ai;

        #region Saves
        private OrbitalitySaveBuilder _saveBuilder = new OrbitalitySaveBuilder();

        public OrbitalitySave GetSave() =>
            _saveBuilder.Build(_planets, _fireController.GetActiveMissiles().ToArray(), _timeDone, _planets.FindIndex(x => x.IsPlayerControlled));
        #endregion


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
        }

        public void StartGame()
        {
            EnsureActive();
            Clear();
            GeneratePlanets();
            SetUpPlayer();
        }

        private void EnsureActive()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
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
            _orbitalityUserMissileInput = new OrbitalityUserMissileInput(_fireController, _transformer);
        }

        private void GeneratePlanets()
        {
            var skinIdxsTemp = Enumerable.Range(0, _planetResources.Skins.Length - 1).ToList();
            var planetCount = Random.Range(_settings.MinPlanets, _settings.MaxPlanets);
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
                planet.PlanetData.SkinId = planet.View.SkinId; //todo:planet is stored twice, reconsider this
                planet.Died += PlanetOnDied;
            }
            _planetBarManager.AddRange(_planets);
        }

        public void Load(OrbitalitySave save)
        {
            EnsureActive();
            OnDisable();
            Clear();

            _timeDone = save.Time;
            //var view = Instantiate(_planetViewPool._prefab);
            //_planets = save.Planets.Select(x => new Planet(x.PlanetData, PreparePlanetView(x.PlanetData, view, x.PlanetData.SkinId)))
            //               .ToList();

            _planets = save.Planets.Select(x => new Planet(x.PlanetData,
                                                           _planetViewPool.Spawn((v) => PreparePlanetView(x.PlanetData, v, x.PlanetData.SkinId))))
                           .ToList();
            UpdatePlanetPositions();

            SetUpPlayer(save.PlayerIdx);


            _fireController.Load(_planets, save.Missiles);
            OnEnable();
            return;
        }

        private PlanetView PreparePlanetView(PlanetData planet, PlanetView view, int skinIdx)
        {
            planet.Cooldown = 3f;//TODO: create weaponry info stuff
            view.transform.parent = transform;
            view.Resources = _planetResources;
            view.Radius = planet.Radius;
            view.SkinId = skinIdx;
            return view;
        }

        private void SetUpPlayer() => SetUpPlayer(Random.Range(0, _planets.Count));
        private void SetUpPlayer(int playerIdx)
        {
            var planet = _planets[playerIdx];
            planet.IsPlayerControlled = true;
            _orbitalityUserMissileInput.Planet = planet;
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
            explosion.Completed -= ExplosionOnCompleted;
            RemovePlanet(planet);
            CheckWinLoose(planet);
        }

        private void RemovePlanet(Planet planet)
        {
            planet.Died -= PlanetOnDied;

            _planetViewPool.Release((PlanetView)planet.View);
            _planetBarManager.Remove(planet);
            _planets.Remove(planet);
            _planetBarManager.Remove(planet);
        }


        //can't use Reset cuz it's taken by mono
        private void Clear()
        {
            _fireController.Clear();

            while (_planets.Count > 0)
                RemovePlanet(_planets[0]);

            while (_planetExplosionPool.Busy.Count > 0)
            {
                var explosion = _planetExplosionPool.Busy[0];
                explosion.Completed -= ExplosionOnCompleted;
            }
        }


        private void CheckWinLoose(Planet deceased)
        {
            if (deceased.IsPlayerControlled)
            {
                OnLoose();
            }

            if (_planets.Count == 1 && _planets[0].IsPlayerControlled)
            {
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
            UpdatePlanetPositions();
            _ai.Tick(_planets, _fireController);
            _timeDone += Time.fixedDeltaTime;
        }

        private void UpdatePlanetPositions()
        {
            foreach (var planet in _planets)
                planet.Position = _motionResolver.Resolve(planet.PlanetData, _timeDone, _settings);
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            PointerHitResolver.Subscribe(_pointerInputCollider, _orbitalityUserMissileInput);
            _planetBarManager.AddRange(_planets);
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            PointerHitResolver.Unsubscribe(_pointerInputCollider, _orbitalityUserMissileInput);
            _planetBarManager.RemoveRange(_planets);
        }
        #endregion
    }
}