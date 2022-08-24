using Combat.Component.Body;
using Combat.Component.Satellite;
using Combat.Component.Ship;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class SatelliteControllser : IController
    {
        public SatelliteControllser(IShip ship, ISatellite satellite, Vector2 position, float rotation)
        {
            _ship = ship;
            _satellite = satellite;
            _position = position;
            _rotation = rotation;
        }

        public virtual void UpdatePhysics(float elapsedTime)
        {
            if (_ship.State == UnitState.Destroyed)
                _satellite.Destroy();
            else if (_ship.State == UnitState.Inactive)
                _satellite.Vanish();
            else
            {
                var requiredPosition = _ship.Body.WorldPosition() + RotationHelpers.Transform(_position, _ship.Body.WorldRotation());
                var requiredRotation = _ship.Body.WorldRotation() + _rotation;
                _satellite.MoveTowards(requiredPosition, requiredRotation, _ship.Body.WorldVelocity(), _velocityFactor, _angularVelocityFactor);
            }
        }

        public void Dispose() { }

        private readonly float _rotation;
        private readonly Vector2 _position;
        private readonly IShip _ship;
        private readonly ISatellite _satellite;

        private const float _velocityFactor = 0.75f;
        private const float _angularVelocityFactor = 3.0f;
    }
}
