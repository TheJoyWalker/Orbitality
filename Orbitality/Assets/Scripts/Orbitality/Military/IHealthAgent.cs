using System;

namespace Orbitality
{
    public class HealthAgentDamageArgs : EventArgs
    {
        public readonly IHealthAgent Victim;
        public readonly IHealthAgent Agressor;
        public readonly int Amount;

        public HealthAgentDamageArgs(IHealthAgent victim, IHealthAgent agressor, int amount)
        {
            Victim = victim;
            Agressor = agressor;
            Amount = amount;
        }
    }

    public class HealthAgentDeathArgs : EventArgs
    {
        public readonly IHealthAgent Victim;
        public readonly IHealthAgent Agressor;
        public readonly int Damage;

        public HealthAgentDeathArgs(IHealthAgent victim, IHealthAgent agressor, int damage)
        {
            Victim = victim;
            Agressor = agressor;
            Damage = damage;
        }
    }

    public delegate void IHealthAgentDeathHandler(HealthAgentDeathArgs args);
    public delegate void IHealthAgentDamageHandler(HealthAgentDamageArgs args);

    public interface IHealthAgent
    {
        int Health { get; }
        int MaxHealth { get; }
        bool IsAlive { get; }

        event IHealthAgentDamageHandler Damaged;
        event IHealthAgentDeathHandler Died;

        void TakeDamage(IHealthAgent source, int amount);
    }
}