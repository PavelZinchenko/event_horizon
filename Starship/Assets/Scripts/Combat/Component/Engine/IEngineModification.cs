using Combat.Component.Mods;

namespace Combat.Component.Engine
{
    public struct EngineData
    {
        public float Velocity;
        public float AngularVelocity;
        public float Propulsion;
        public float TurnRate;
        public float Throttle;
        public float Deceleration;
        public bool HasCourse;
        public float Course;
    }

    public interface IEngineModification : IModification<EngineData> {}
}
