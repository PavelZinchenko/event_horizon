using System;

namespace Combat.Component.Controller
{
    public interface IController : IDisposable
    {
        void UpdatePhysics(float elapsedTime);
    }
}
