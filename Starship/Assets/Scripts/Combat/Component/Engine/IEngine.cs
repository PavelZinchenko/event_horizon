using Combat.Component.Body;
using Combat.Component.Mods;

namespace Combat.Component.Engine
{
    public interface IEngine
    {
        float MaxVelocity { get; }
        float MaxAngularVelocity { get; }
        float Propulsion { get; }
        float TurnRate { get; }

        float? Course { get; set; }
        float Throttle { get; set; }

        Modifications<EngineData> Modifications { get; }

        void Update(float elapsedTime, IBody body);
    }
}
