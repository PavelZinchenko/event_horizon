using System;
using Combat.Component.Engine;
using Combat.Component.Features;
using Combat.Component.Stats;
using Combat.Component.Systems;
using Combat.Component.Triggers;

namespace Combat.Component.Ship.Effects
{
    public interface IShipEffect : IDisposable
    {
        bool IsAlive { get; }

        void UpdatePhysics(IShip ship, float elapsedTime);
        void UpdateView(IShip ship, float elapsedTime);

        IEngineModification EngineModification { get; }
        IFeaturesModification FeaturesModification { get; }
        ISystemsModification SystemsModification { get; }
        IStatsModification StatsModification { get; }
        IUnitAction UnitAction { get; }
    }
}