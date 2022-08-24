using Combat.Component.Body;
using Combat.Component.Mods;

namespace Combat.Component.Engine
{
    public class NullEngine : IEngine
    {
        public float MaxVelocity { get { return 0; } }
        public float MaxAngularVelocity { get { return 0; } }
        public float Propulsion { get { return 0; } }
        public float TurnRate { get { return 0; } }

        public float? Course { get { return null; } set { } }
        public float Throttle { get { return 0; } set { } }

        public Modifications<EngineData> Modifications { get { return _modifications; } }

        public void Update(float elapsedTime, IBody body)
        {
            ApplyDeceleration(body, elapsedTime);
            ApplyAngularDeceleration(body, elapsedTime);
        }

        private void ApplyDeceleration(IBody body, float elapsedTime)
        {
            var velocity = body.Velocity;
            if (velocity.sqrMagnitude < 0.001f)
                return;

            body.ApplyAcceleration(-velocity * elapsedTime);
        }

        private void ApplyAngularDeceleration(IBody body, float elapsedTime)
        {
            var angularVelocity = body.AngularVelocity;
            if (angularVelocity < 0.001f && angularVelocity > -0.001f)
                return;

            body.ApplyAngularAcceleration(-angularVelocity * elapsedTime);
        }

        private readonly Modifications<EngineData> _modifications = new Modifications<EngineData>();
    }
}
