using System;
using Combat.Component.Features;
using Combat.Component.Engine;
using Combat.Component.Stats;
using Combat.Component.Triggers;
using GameDatabase.Model;

namespace Combat.Component.Systems
{
    public enum SystemEventType
    {
        DamageTaken,
    }

    public interface ISystem : IDisposable
    {
        bool Enabled { get; set; }
        bool Active { get; set; }

        float ActivationCost { get; }
        bool CanBeActivated { get; }
        float Cooldown { get; }

        int KeyBinding { get; }
        SpriteId ControlButtonIcon { get; }

        IEngineModification EngineModification { get; }
        IFeaturesModification FeaturesModification { get; }
        ISystemsModification SystemsModification { get; }
        IStatsModification StatsModification { get; }
        IUnitAction UnitAction { get; }

        void UpdatePhysics(float elapsedTime);
        void UpdateView(float elapsedTime);

        void OnEvent(SystemEventType eventType);
    }
}
