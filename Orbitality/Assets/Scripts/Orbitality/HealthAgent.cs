using System;
using UnityEngine;

namespace Orbitality
{
    public class HealthAgent : IHealthAgent
    {
        public bool IsAlive => _health > 0;
        private int _health;
        private IHealthAgent _delegator;
        public int MaxHealth { get; }
        public int Health => _health;

        public event IHealthAgentDamageHandler Damaged;
        public event IHealthAgentDeathHandler Died;

        /// <summary>
        /// care this class is intended for delegation
        /// </summary>
        /// <param name="delegator">delegator</param>
        /// <param name="maxHealth"></param>
        public HealthAgent(IHealthAgent delegator, int maxHealth) : this(delegator, maxHealth, maxHealth) { }
        public HealthAgent(IHealthAgent delegator, int maxHealth, int currentHealth)
        {
            if (delegator == null)
                throw new ArgumentException("HealthAgent is intended for delegation, plz provide a valid delegator");
            _delegator = delegator;
            MaxHealth = maxHealth;
            _health = currentHealth;
        }

        public void TakeDamage(IHealthAgent source, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Damage can not be < 0");
            if (!IsAlive)
                Debug.Log("hitting dead");

            _health = Mathf.Max(0, _health - amount);
            OnDamaged(new HealthAgentDamageArgs(_delegator, source, amount));
        }

        private void Die() => TakeDamage(_delegator, Health);
        protected virtual void OnDamaged(HealthAgentDamageArgs args)
        {
            Debug.Log($"TookDamage: {args.Amount}, {_health} left");
            Damaged?.Invoke(args);

            if (!IsAlive)
                OnDied(new HealthAgentDeathArgs(_delegator, args.Agressor, args.Amount));
        }
        protected virtual void OnDied(HealthAgentDeathArgs args)
        {
            Debug.Log("Died");
            Died?.Invoke(args);
        }
    }
}