using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Orbitality
{
    public class Collision2DAgent : MonoBehaviour, ICollision2DAgent
    {
        public event EventHandler<Collision2D> Entered;
        public event EventHandler<Collider2D> Staying;
        public event EventHandler<Collision2D> Exited;
        [UsedImplicitly]
        void OnCollisionEnter2D(Collision2D collision2D)
        {
            //Debug.Log("OnCollisionEnter2D");
            Entered?.Invoke(this, collision2D);
        }

        [UsedImplicitly]
        void OnTriggerStay2D(Collider2D collision2D) => Staying?.Invoke(this, collision2D);
        [UsedImplicitly]
        void OnCollisionExit2D(Collision2D collision2D) => Exited?.Invoke(this, collision2D);
    }
}