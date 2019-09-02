using System;
using UnityEngine;

namespace Orbitality
{
    [SerializeField]
    public class Planet : IHealthAgent
    {
        public bool IsPlayerControlled { get; set; }

        public PlanetData PlanetData { get; }

        public Vector3 WorldPosition => View.Position;
        public Vector3 Position
        {
            get => View.Position;
            set => View.Position = value;
        }
        public IPlanetView View { get; }


        private readonly ChargeManager _chargeManager;
        private readonly HealthAgent _healthAgent;
        public Vector3 BarPosition => View.BarPosition;

        public Planet(PlanetData data, IPlanetView view)
        {
            PlanetData = data;
            View = view;
            view.Owner = this;
            _healthAgent = new HealthAgent(this, data.MaxHealth);
            _chargeManager = new ChargeManager(data.Cooldown, startCharged: false);
        }

        #region hp stuff
        public int MaxHealth => _healthAgent.MaxHealth;
        public int Health => _healthAgent.Health;
        public bool IsAlive => _healthAgent.IsAlive;

        public event IHealthAgentDamageHandler Damaged
        {
            add => _healthAgent.Damaged += value;
            remove => _healthAgent.Damaged -= value;
        }
        public event IHealthAgentDeathHandler Died
        {
            add => _healthAgent.Died += value;
            remove => _healthAgent.Died -= value;
        }

        public void TakeDamage(IHealthAgent source, int amount) => _healthAgent.TakeDamage(source, amount);
        #endregion

        #region cooldown
        public void RegisterShot() => _chargeManager.Discharge();
        public float ChargeTime => _chargeManager.ChargeTime;
        public float Cooldown => _chargeManager.Cooldown;
        public float ChargePercent => _chargeManager.ChargePercent;
        public bool CanShoot => _chargeManager.IsCharged;
        #endregion
    }
}