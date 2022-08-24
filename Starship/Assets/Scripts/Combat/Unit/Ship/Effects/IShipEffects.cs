using System;
using System.Collections.Generic;

namespace Combat.Component.Ship.Effects
{
    public interface IShipEffects : IDisposable
    {
        bool TryAdd(IShipEffect effect);
        IList<IShipEffect> All { get; }

        void UpdatePhysics(float elapsedTime);
        void UpdateView(float elapsedTime);
    }
}
