using System;
using Combat.Component.Collider;
using Combat.Component.Mods;
using Combat.Component.View;
using UnityEngine;

namespace Combat.Component.Features
{
    public enum TargetPriority { None, Low, Normal, High }

    public interface IFeatures : IDisposable
    {
        TargetPriority TargetPriority { get; }
        Color Color { get; }

        void UpdatePhysics(float elapsedTime, ICollider collider);
        void UpdateView(float elapsedTime, IView view);

        Modifications<FeaturesData> Modifications { get; }
    }
}
