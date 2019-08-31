using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Orbitality;
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
        #region Pools
        private SimpleMonoPool<PlanetView> _planetPool;
        private Dictionary<int, SimpleMonoPool<Missile>> _missilePools = new Dictionary<int, SimpleMonoPool<Missile>>();
        #endregion
        #endregion

        #region ScriptableObjects
        [SerializeField] private OrbitalityMonoSettings _settings = default;
        [SerializeField] private OrbitalityPlanetResources _planetResources = default;
        [SerializeField] private OrbitalityMissileResources _missileResources = default;
        #endregion

        [SerializeField] private Collider _pointerInputCollider = default;
        private List<Planet> _planets = new List<Planet>();
        [SerializeField] private float _timeDone;

        private void Awake()
        {
            Initialize();
            GeneratePlanets();
            _orbitalityUserInput = new OrbitalityUserInput(_aimController, _transformer, _planets[2]);
            Debug.Log($"Create {_planets.Count} planets.");
        }

        private void Initialize()
        {
            _transformer = new WorldToLocalTransformer(transform);
            _planetPool = new SimpleMonoPool<PlanetView>(_planetResources.Prefab);
            for (var i = 0; i < _missileResources.Missiles.Length; i++)
            {
                _missilePools[i] = new SimpleMonoPool<Missile>(_missileResources.Missiles[i]);
            }
            _aimController = new AimController(_missilePools, transform);
        }

        private void GeneratePlanets()
        {
            _planets = _planetGenerator.Generate(_settings.MaxPlanets, _settings)
                                       .Select(x => new Planet(x, _planetPool.Spawn(v =>
                                                                                    {
                                                                                        v.transform.parent = transform;
                                                                                        v.Resources = _planetResources;
                                                                                        v.Radius = x.Radius;
                                                                                        v.SkinId = GetFreeSkinId();
                                                                                    }))).ToList();
        }


        private void FixedUpdate()
        {
            foreach (var planet in _planets)
                planet.Position = _motionResolver.Resolve(planet.PlanetData, _timeDone, _settings);

            _timeDone += Time.fixedDeltaTime;
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

        private void OnEnable() => PointerHitResolver.Subscribe(_pointerInputCollider, _orbitalityUserInput);
        private void OnDisable() => PointerHitResolver.Unsubscribe(_pointerInputCollider, _orbitalityUserInput);
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

        public Missile CreateMissile(int type, Planet planet)
        {
            var missile = _pools[type].Spawn((x) => x.SpawnReset());
            missile.transform.parent = _parent;
            missile.Id = type;
            missile.Die(20f);
            missile.Owner = planet;
            _missiles.Add(missile);
            return missile;
        }

        public void AimAt(Missile missile, Vector2 point)
        {
            //missile.transform.localPosition = missile.Owner.Position;
            missile.transform.localPosition = (Vector2)missile.Owner.Position +
                                              ((point - (Vector2)missile.Owner.Position).normalized * missile.Owner.PlanetData.Radius * .6f);
            missile.LookAt(point);
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
        public void OnPointerDown(Vector3 worldHitPoint)
        {
            _missile = _aimController.CreateMissile(Planet.PlanetData.WeaponType, Planet);
        }

        public void OnPointerStay(Vector3 worldHitPoint)
        {
            _aimController.AimAt(_missile, _transformer.Transform(worldHitPoint));
        }

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