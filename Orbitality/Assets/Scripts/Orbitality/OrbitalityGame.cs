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
            _orbitalityUserInput = new OrbitalityUserInput(_fireController, _transformer, _planets[playerIdx]);
        }

        private void Initialize()
        {
            _transformer = new WorldToLocalTransformer(transform);
            _planetViewPool = new SimpleMonoPool<PlanetView>(_planetResources.Prefab);
            _planetExplosionPool = new SimpleMonoPool<ParticleExplosionController>(_planetResources.ExplosionPrefab);
            _ai = new OrbitalityAI();
            for (var i = 0; i < _missileResources.Missiles.Length; i++)
            {
                _missilePools[i] = new SimpleMonoPool<Missile>(_missileResources.Missiles[i]);
            }
            _fireController = new FireController(_missilePools, transform);
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
            explosion.Explode();
            explosion.Completed += ExplosionOnCompleted;
            _planetViewPool.Release((PlanetView)planet.View);
            _planets.Remove(planet);
            _planetBarManager.Remove(planet);
        }

        private void ExplosionOnCompleted(ParticleExplosionController obj) => _planetExplosionPool.Release(obj);

        private void PreparePlanetView(PlanetData planet, PlanetView view, int skinIdx)
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
    }

    public class OrbitalityAI
    {
        //todo: optimize to reduce GC
        public void Tick(List<Planet> planets, FireController fireController)
        {
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

    public class WorldToLocalTransformer
    {
        public WorldToLocalTransformer(Transform targetSpaceTransform) => _targetSpaceTransform = targetSpaceTransform;
        private Transform _targetSpaceTransform;
        public Vector3 Transform(Vector3 worldPoint) => _targetSpaceTransform.InverseTransformPoint(worldPoint);
    }

    public delegate void HitHandler(Planet source, Planet target, Missile missile);
    public class FireController
    {
        public event HitHandler Hit;
        private readonly IDictionary<int, SimpleMonoPool<Missile>> _pools;
        private readonly List<Missile> _missiles = new List<Missile>();
        private Transform _parent;
        public FireController(IDictionary<int, SimpleMonoPool<Missile>> pools, Transform parent)
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
            for (int i = 0; i < e.contactCount; i++)
            {
                //todo: split view and logic!
                var planet = e.GetContact(i).collider.GetComponent<PlanetView>();
                if (planet != null)
                {
                    planet.Owner.TakeDamage(missile.Owner, missile.Damage);
                }
            }
        }
    }

    public class OrbitalityUserInput : IHitReceiver
    {
        private readonly FireController _fireController;
        public Planet Planet { get; set; }//could this change in future?
        private WorldToLocalTransformer _transformer;
        private Missile _missile;

        public OrbitalityUserInput(FireController fireController, WorldToLocalTransformer transformer, Planet planet)
        {
            _fireController = fireController;
            _transformer = transformer;
            Planet = planet;
        }
        public void OnPointerDown(Vector3 worldHitPoint)
        {
            //todo: player can shot without cooldown
            //if (Planet.CanShoot)
            _missile = _fireController.CreateMissile(Planet.PlanetData.WeaponType, Planet, GetAimPoint(worldHitPoint));
        }

        public void OnPointerStay(Vector3 worldHitPoint)
        {
            if (_missile == null)
                return;

            _fireController.AimAt(_missile, GetAimPoint(worldHitPoint));
        }

        private Vector3 GetAimPoint(Vector3 worldHitPoint) => _transformer.Transform(worldHitPoint);

        public void OnPointerUp(Vector3 worldHitPoint)
        {
            if (_missile == null)
                return;

            _fireController.Fire(_missile);
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

        public int MaxHealth = 100;
    }
}