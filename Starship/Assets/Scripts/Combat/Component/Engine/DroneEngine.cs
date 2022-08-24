using Combat.Component.Body;
using Combat.Component.Mods;
using UnityEngine;

namespace Combat.Component.Engine
{
    public class DroneEngine : IEngine
    {
        public DroneEngine(float propulsion, float turnRate, float velocity, float angularVelocity, float maxVelocity, float maxAngularVelocity)
        {
            _propulsion = propulsion;
            _turnRate = turnRate;
            _velocity = velocity;
            _angularVelocity = angularVelocity;
            _maxVelocity = maxVelocity;
            _maxAngularVelocity = maxAngularVelocity;
            UpdateData();
        }

        public float MaxVelocity { get { return _engineData.Velocity; } }
        public float MaxAngularVelocity { get { return _engineData.AngularVelocity; } }
        public float Propulsion { get { return _engineData.Propulsion; } }
        public float TurnRate { get { return _engineData.TurnRate; } }

        public float? Course
        {
            get
            {
                if (_engineData.HasCourse)
                    return _engineData.Course;
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _engineData.HasCourse = true;
                    _engineData.Course = value.Value;
                }
                else
                {
                    _engineData.HasCourse = false;
                }
            }
        }

        public float Throttle { get { return _engineData.Throttle; } set { _engineData.Throttle = value; } }

        public Modifications<EngineData> Modifications { get { return _modifications; } }

        public void Update(float elapsedTime, IBody body)
        {
            UpdateData();

            if (Throttle > 0.01f)
                ApplyAcceleration(body, elapsedTime);
            if (_engineData.Deceleration > 0)
                ApplyDeceleration(body, elapsedTime);

            if (_engineData.HasCourse)
                ApplyAngularAcceleration(body, elapsedTime);
            else if (Mathf.Abs(body.AngularVelocity) > 0.01f)
                ApplyAngularDeceleration(body, elapsedTime);
        }

        private void UpdateData()
        {
            _engineData.AngularVelocity = _angularVelocity;
            _engineData.Velocity = _velocity;
            _engineData.TurnRate = _turnRate;
            _engineData.Propulsion = _propulsion;
            _engineData.Deceleration = 0;

            _modifications.Apply(ref _engineData);

            if (_engineData.Velocity > _maxVelocity)
                _engineData.Velocity = _maxVelocity;
            if (_engineData.AngularVelocity > _maxAngularVelocity)
                _engineData.AngularVelocity = _maxAngularVelocity;
        }

        private void ApplyAcceleration(IBody body, float elapsedTime)
        {
            var acceleration = Throttle * Propulsion;
            var forward = RotationHelpers.Direction(body.Rotation);
            var velocity = body.Velocity;
            var forwardVelocity = Vector2.Dot(velocity, forward);

            if (forwardVelocity >= MaxVelocity)
                return;

            var requiredVelocity = Mathf.Max(forwardVelocity, MaxVelocity) * forward;
            var direction = (requiredVelocity - velocity).normalized;

            body.ApplyAcceleration(acceleration * elapsedTime * direction);
        }

        private void ApplyDeceleration(IBody body, float elapsedTime)
        {
            var velocity = body.Velocity;
            if (velocity.magnitude < 0.001f)
                return;

            var direction = velocity.normalized;
            body.ApplyAcceleration(-_engineData.Deceleration * elapsedTime * direction);
        }

        private void ApplyAngularAcceleration(IBody body, float elapsedTime)
        {
            var angularVelocity = body.AngularVelocity;
            var acceleration = 0f;

            var minDeltaAngle = Mathf.DeltaAngle(body.Rotation, _engineData.Course);

            var deltaAngle = 0f;
            if (minDeltaAngle > 0 && angularVelocity < 0)
                deltaAngle = 360 - minDeltaAngle;
            else if (minDeltaAngle < 0 && angularVelocity > 0)
                deltaAngle = 360 + minDeltaAngle;
            else
                deltaAngle = Mathf.Abs(minDeltaAngle);

            if (deltaAngle < 120f && deltaAngle < angularVelocity * angularVelocity / TurnRate)
                acceleration = Mathf.Clamp(-angularVelocity, -TurnRate * elapsedTime, TurnRate * elapsedTime);
            else if (minDeltaAngle < 0 && angularVelocity > -MaxAngularVelocity || minDeltaAngle > 0 && angularVelocity < MaxAngularVelocity)
                acceleration = Mathf.Clamp(minDeltaAngle, -TurnRate * elapsedTime, TurnRate * elapsedTime);
            else
                return;

            body.ApplyAngularAcceleration(acceleration);
        }

        private void ApplyAngularDeceleration(IBody body, float elapsedTime)
        {
            var acceleration = Mathf.Clamp(-body.AngularVelocity, -TurnRate * elapsedTime, TurnRate * elapsedTime);
            body.ApplyAngularAcceleration(acceleration);
        }

        private EngineData _engineData;

        private readonly float _propulsion;
        private readonly float _turnRate;
        private readonly float _velocity;
        private readonly float _angularVelocity;
        private readonly float _maxAngularVelocity;
        private readonly float _maxVelocity;
        private readonly Modifications<EngineData> _modifications = new Modifications<EngineData>();
    }
}
