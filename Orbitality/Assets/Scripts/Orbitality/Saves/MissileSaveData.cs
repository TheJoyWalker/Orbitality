using System;
using UnityEngine;

namespace Orbitality.Saves
{
    /// <summary>
    /// Todo: replace with a custom serializer
    /// </summary>
    [Serializable]
    public class SerializableVector2
    {
        public float x;
        public float y;

        public SerializableVector2()
        {
            
        }

        public SerializableVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator Vector2(SerializableVector2 v) => new Vector2(v.x, v.y);
        public static implicit operator SerializableVector2(Vector2 v) => new SerializableVector2(v.x, v.y);
    }

    public class SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Quaternion(SerializableQuaternion q) => new Quaternion(q.x, q.y, q.z, q.w);
        public static implicit operator SerializableQuaternion(Quaternion q) => new SerializableQuaternion(q.x, q.y, q.z, q.w);
    }
    [Serializable]
    public class MissileSaveData
    {
        public SerializableVector2 Position;
        public int OwnerIndex;
        public int TypeId;
        public SerializableQuaternion Rotation;
        public SerializableVector2 Velocity;
        public float AngularVelocity;
        public int Damage;
    }
}