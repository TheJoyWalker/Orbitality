using System;
using UnityEngine;

namespace Orbitality
{
    [SerializeField]
    public class Planet
    {
        private const int InitialHealth = 100;
        private readonly ChargeManager _chargeManager;

        private int _health = InitialHealth;

        public Planet(PlanetData data, IPlanetView view)
        {
            PlanetData = data;
            View = view;
            _chargeManager = new ChargeManager(data.Cooldown, true);
        }

        public Vector3 WorldPosition => View.Position;

        public Vector3 Position
        {
            get => View.Position;
            set => View.Position = value;
        }

        [SerializeField] public PlanetData PlanetData { get; }
        public IPlanetView View { get; }

        public int MaxHealth => InitialHealth;

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

        public bool IsPlayerControlled { get; set; }

        public Vector3 BarPosition => View.BarPosition;
        public event EventHandler<int> Damaged;
        public event EventHandler<int> Healed;
        public event EventHandler Died;


        public void TakeDamage(int amount) => Health -= amount;


        private void Die()
        {
            OnDied();
        }

        protected virtual void OnDamaged(int e)
        {
            Damaged?.Invoke(this, e);
            if (Health == 0)
                Die();
        }

        protected virtual void OnHealed(int e) => Healed?.Invoke(this, e);
        protected virtual void OnDied() => Died?.Invoke(this, EventArgs.Empty);

#region cooldown
        public void RegisterShot() => _chargeManager.Discharge();
        public float ChargeTime => _chargeManager.ChargeTime;
        public float Cooldown => _chargeManager.Cooldown;
        public float ChargePercent => _chargeManager.ChargePercent;
        public bool CanShoot => _chargeManager.IsCharged;
#endregion
    }
}