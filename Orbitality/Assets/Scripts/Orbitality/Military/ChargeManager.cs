using UnityEngine;

namespace Orbitality
{
    public class ChargeManager
    {
        private float _lastShotTime;

        public ChargeManager(float chargeTime) : this(chargeTime, true)
        {
        }

        public ChargeManager(float chargeTime, bool startCharged)
        {
            ChargeTime = chargeTime;
            if (startCharged)
                _lastShotTime = -chargeTime;
        }

        public float ChargeTime { get; }
        /// <summary>
        ///     Time till next shot
        /// </summary>
        public float Cooldown => Mathf.Max(0, _lastShotTime + ChargeTime - Time.timeSinceLevelLoad);

        public float ChargePercent => 1 - Cooldown / ChargeTime;
        public bool IsCharged => Cooldown <= 0f;
        public void Discharge() => _lastShotTime = Time.timeSinceLevelLoad;
    }
}