using System.Collections.Generic;
using UnityEngine;

namespace Orbitality.Saves
{
    public class MissileSaveBuilder
    {
        public MissileSaveData Build(IList<Planet> planets, Missile missile)
        {
            return new MissileSaveData()
            {
                TypeId = missile.TypeId,
                OwnerIndex = planets.IndexOf(missile.Owner),
                Damage = missile.Damage,
                Position = (Vector2)missile.transform.localPosition,
                Rotation = missile.transform.localRotation,
                TargetPoint = missile.TargetPoint,
                Velocity = missile.Rigidbody2D.velocity,
                AngularVelocity = missile.Rigidbody2D.angularVelocity,
            };
        }
    }
}
