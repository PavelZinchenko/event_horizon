using Combat.Component.Unit;
using UnityEngine;

namespace Combat.Component.Controller
{
    public class BeamController : IController
    {
        public BeamController(IUnit unit, float defaultSpread, float rotationOffset)
        {
            _unit = unit;
            _defaultSpread = defaultSpread;
            _rotationOffset = rotationOffset;
        }

        public void Dispose() { }

        public void UpdatePhysics(float elapsedTime)
        {
            _unit.Body.Turn((Random.value - 0.5f) * _defaultSpread + _rotationOffset);
        }

        private readonly float _rotationOffset;
        private readonly float _defaultSpread;
        private readonly IUnit _unit;
    }
}
