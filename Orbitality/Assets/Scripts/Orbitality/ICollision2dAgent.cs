using System;
using UnityEngine;

namespace Orbitality
{
    public interface ICollision2DAgent
    {
        event EventHandler<Collision2D> Entered;
        event EventHandler<Collision2D> Exited;
        event EventHandler<Collider2D> Staying;
    }
}