using System;
using UnityEngine;

namespace Orbitality
{
    [SerializeField]
    public class Planet
    {
        public event EventHandler<int> Damaged;
        public event EventHandler<int> Healed;
        public event EventHandler Died;
        private readonly float _cooldown;

        public Vector3 Position
        {
            get => View.Position;
            set => View.Position = value;
        }
        public Planet(PlanetData data, float cooldown, IPlanetView view)
        {
            _cooldown = cooldown;
            PlanetData = data;
            View = view;
        }

        [SerializeField] public PlanetData PlanetData { get; }
        public IPlanetView View { get; }

        public int MaxHealth => 100;
        public int Health
        {
            get => _health;
            private set
            {
                var delta = value - _health;
                _health = Mathf.Max(0, value);

                if (delta < 0)
                    OnDamaged(-delta);
                else if (delta > 0)
                    OnHealed(delta);
            }
        }

        private float _lastShotTime;
        private int _health;

        public void RegisterShot() => _lastShotTime = Time.timeSinceLevelLoad;
        public void TakeDamage(int amount) => Health -= amount;

        public float Cooldown => Time.timeSinceLevelLoad - (_lastShotTime + _cooldown);
        public bool CanShoot => Cooldown >= 0f;

        private void Die() { OnDied(); }

        protected virtual void OnDamaged(int e)
        {
            Damaged?.Invoke(this, e);
            if (Health == 0)
                Die();
        }

        public bool IsPlayerControlled { get; set; }

        protected virtual void OnHealed(int e) => Healed?.Invoke(this, e);
        protected virtual void OnDied() => Died?.Invoke(this, EventArgs.Empty);
    }
}