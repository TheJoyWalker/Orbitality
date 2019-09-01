using System;
using System.Collections.Generic;
using Pools;
using UnityEngine;

namespace Orbitality
{
    public class FireController
    {
        private readonly IDictionary<int, SimpleMonoPool<Missile>> _missilePools;
        private readonly SimpleMonoPool<ParticleExplosionController> _explosionPool;
        private readonly List<Missile> _missiles = new List<Missile>();
        private Transform _parent;
        public FireController(IDictionary<int, SimpleMonoPool<Missile>> missilePools, SimpleMonoPool<ParticleExplosionController> explosionPool, Transform parent)
        {
            _explosionPool = explosionPool;
            _missilePools = missilePools;
            _parent = parent;
        }

        public Missile CreateMissile(int type, Planet planet, Vector2 aimPoint)
        {
            var missile = _missilePools[type].Spawn((x) =>
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
            _missilePools[missile.Id].Release(missile);
            _missiles.Remove(missile);
            var explosion = _explosionPool.Spawn(x =>
                                                 {
                                                     x.transform.SetParent(_parent);
                                                     x.transform.position = missile.transform.position;
                                                 });
            explosion.Completed += ExplosionOnCompleted;
            explosion.Explode();

        }

        private void ExplosionOnCompleted(ParticleExplosionController obj) => _explosionPool.Release(obj);

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
}