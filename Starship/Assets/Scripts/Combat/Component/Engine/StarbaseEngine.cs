using Combat.Component.Body;
using Combat.Component.Mods;
using UnityEngine;

namespace Combat.Component.Engine
{
    public class StarbaseEngine : IEngine
    {
        public StarbaseEngine(float angularVelocity)
        {
            _angularVelocity = angularVelocity;
        }

        public float MaxVelocity { get { return 0; } }
        public float MaxAngularVelocity { get { return _angularVelocity > 0 ? _angularVelocity : -_angularVelocity; } }
        public float Propulsion { get { return 0; } }
        public float TurnRate { get { return 0; } }

        public float? Course { get { return null; } set {} }
        public float Throttle { get { return 0; } set {} }

        public Modifications<EngineData> Modifications { get { return _modifications; } }

        public void Update(float elapsedTime, IBody body)
        {
            ApplyDeceleration(body, elapsedTime);
            ApplyAngularAcceleration(body, elapsedTime);
        }

        private void ApplyDeceleration(IBody body, float elapsedTime)
        {
            var velocity = body.Velocity;
            if (velocity.sqrMagnitude < 0.001f)
                return;

            body.ApplyAcceleration(-velocity*elapsedTime);
        }

        private void ApplyAngularAcceleration(IBody body, float elapsedTime)
        {
            var angularVelocity = body.AngularVelocity;
            var acceleration = Mathf.DeltaAngle(angularVelocity, _angularVelocity) * elapsedTime;
            if (acceleration < 0.001f && acceleration > -0.001f)
                return;

            body.ApplyAngularAcceleration(acceleration);
        }

        private readonly float _angularVelocity;
        private readonly Modifications<EngineData> _modifications = new Modifications<EngineData>();
    }
}
