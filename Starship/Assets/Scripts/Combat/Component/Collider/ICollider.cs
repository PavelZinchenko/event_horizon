using System;
using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Component.Collider
{
    public interface ICollider : IDisposable
    {
        bool Enabled { get; set; }

        IUnit Unit { get; set; }
        float MaxRange { get; set; }
        //bool IsTrigger { get; set; }

        IUnit ActiveCollision { get; }
        Vector2 LastContactPoint { get; }

        void UpdatePhysics(float elapsedTime);
    }
}
