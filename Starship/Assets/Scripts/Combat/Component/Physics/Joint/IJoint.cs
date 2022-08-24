using System;

namespace Combat.Component.Physics.Joint
{
    public interface IJoint : IDisposable
    {
        bool IsActive { get; }
    }
}
