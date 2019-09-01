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
        private AimController _aimController;
        private OrbitalityAI _ai;
        #region Pools
        private SimpleMonoPool<PlanetView> _planetViewPool;
        private Dictionary<int, SimpleMonoPool<Missile>> _missilePools = new Dictionary<int, SimpleMonoPool<Missile>>();
        #endregion
        #endregion

        #region ScriptableObjects
        [SerializeField] private OrbitalityMonoSettings _settings = default;
        [SerializeField] private OrbitalityPlanetResources _planetResources = default;
        [SerializeField] private OrbitalityMissileResources _missileResources = default;
        #endregion
        [SerializeField] private PlanetBarManager _planetBarManager;

        [SerializeField] private Collider _pointerInputCollider = default;
        private List<Planet> _planets = new List<Planet>();
        [SerializeField] private float _timeDone;

        private void Awake()
        {
            Initialize();
            GeneratePlanets();
            SetPlayer();
            Debug.Log($"Create {_planets.Count} planets.");
        }

        private void SetPlayer()
        {
            var playerIdx = Random.Range(0, _planets.Count);
            _planets[playerIdx].IsPlayerControlled = true;
            _orbitalityUserInput = new OrbitalityUserInput(_aimController, _transformer, _planets[playerIdx]);
        }

        private void Initialize()
        {
            _transformer = new WorldToLocalTransformer(transform);
            _planetViewPool = new SimpleMonoPool<PlanetView>(_planetResources.Prefab);
            _ai = new OrbitalityAI();
            for (var i = 0; i < _missileResources.Missiles.Length; i++)
            {
                _missilePools[i] = new SimpleMonoPool<Missile>(_missileResources.Missiles[i]);
            }
            _aimController = new AimController(_missilePools, transform);
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
                                                   new Planet(x, _planetViewPool.Spawn(v => PreparePlanet(x, v, skinIdxs[idx])))).ToList();


        }

        private void PreparePlanet(PlanetData planet, PlanetView view, int skinIdx)
        {
            planet.Cooldown = 3f;//TODO: create weaponry info stuff
            view.transform.parent = transform;
            view.Resources = _planetResources;
            view.Radius = planet.Radius;
            view.SkinId = skinIdx;
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            foreach (var planet in _planets)
                planet.Position = _motionResolver.Resolve(planet.PlanetData, _timeDone, _settings);
            _ai.Tick(_planets, _aimController);
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
    }

    public class OrbitalityAI
    {
        //todo: optimize to reduce GC
        public void Tick(List<Planet> planets, AimController aimController)
        {
            foreach (var planet in planets.Where(x => x.CanShoot && !x.IsPlayerControlled))
            {
                aimController.Fire(planet.PlanetData.WeaponType, planet, FindAim(planet, planets).Position);
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

    public class WorldToLocalTransformer
    {
        public WorldToLocalTransformer(Transform targetSpaceTransform) => _targetSpaceTransform = targetSpaceTransform;
        private Transform _targetSpaceTransform;
        public Vector3 Transform(Vector3 worldPoint) => _targetSpaceTransform.InverseTransformPoint(worldPoint);
    }

    public class AimController
    {
        private readonly IDictionary<int, SimpleMonoPool<Missile>> _pools;
        private readonly List<Missile> _missiles = new List<Missile>();
        private Transform _parent;
        public AimController(IDictionary<int, SimpleMonoPool<Missile>> pools, Transform parent)
        {
            _pools = pools;
            _parent = parent;
        }

        public Missile CreateMissile(int type, Planet planet, Vector2 aimPoint)
        {
            var missile = _pools[type].Spawn((x) =>
                                             {
                                                 x.transform.parent = _parent;
                                                 x.SpawnReset();
                                                 x.Id = type;
                                                 x.Owner = planet;
                                                 AimAt(x, aimPoint);
                                             });
            missile.Die(20f);
            _missiles.Add(missile);
            planet.RegisterShot();
            return missile;
        }

        public void AimAt(Missile missile, Vector2 point)
        {
            //missile.transform.localPosition = missile.Owner.Position;
            missile.transform.localPosition = (Vector2)missile.Owner.Position +
                                              ((point - (Vector2)missile.Owner.Position).normalized * missile.Owner.PlanetData.Radius * .6f);
            missile.LookAt(point);
        }

        public void Fire(int type, Planet planet, Vector2 aimPoint)
        {
            var missile = CreateMissile(type, planet, aimPoint);
            Fire(missile);
        }

        public void Fire(Missile missile)
        {
            missile.Fire();
            missile.Entered += MissileOnEntered;
            missile.Died += MissileOnDied;
        }

        private void MissileOnDied(object sender, EventArgs e)
        {
            var missile = (Missile)sender;
            missile.Died -= MissileOnDied;
            missile.Entered -= MissileOnEntered;
            _pools[missile.Id].Release(missile);
            _missiles.Remove(missile);
        }

        private void MissileOnEntered(object sender, Collision2D e)
        {
            var missile = (Missile)sender;
            missile.Entered += MissileOnEntered;
            missile.Die();
        }
    }

    public class OrbitalityUserInput : IHitReceiver
    {
        private readonly AimController _aimController;
        public Planet Planet { get; set; }//could this change in future?
        private WorldToLocalTransformer _transformer;
        private Missile _missile;

        public OrbitalityUserInput(AimController aimController, WorldToLocalTransformer transformer, Planet planet)
        {
            _aimController = aimController;
            _transformer = transformer;
            Planet = planet;
        }
        public void OnPointerDown(Vector3 worldHitPoint) => _missile = _aimController.CreateMissile(Planet.PlanetData.WeaponType, Planet, GetAimPoint(worldHitPoint));

        public void OnPointerStay(Vector3 worldHitPoint) => _aimController.AimAt(_missile, GetAimPoint(worldHitPoint));

        private Vector3 GetAimPoint(Vector3 worldHitPoint) => _transformer.Transform(worldHitPoint);

        public void OnPointerUp(Vector3 worldHitPoint)
        {
            _aimController.Fire(_missile);
            _missile = null;
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
        public int WeaponType;
    }
}