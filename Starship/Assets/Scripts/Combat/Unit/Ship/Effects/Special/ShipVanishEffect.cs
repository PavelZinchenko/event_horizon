using Combat.Component.Engine;
using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Ship.Effects;
using Combat.Component.Stats;
using Combat.Component.Systems;
using Combat.Component.Triggers;
using Combat.Scene;

namespace Combat.Unit.Ship.Effects.Special
{
    public class ShipVanishEffect : IShipEffect
    {
        public ShipVanishEffect(float maxDistance, IScene scene)
        {
            _scene = scene;
            _maxDistance = maxDistance;
        }

        public bool IsAlive { get { return true; } }

        public void UpdatePhysics(IShip ship, float elapsedTime)
        {
            if (!ship.IsActive())
                return;
            var player = _scene.PlayerShip;
            if (!player.IsActive())
                return;

            if (ship.Body.Position.SqrDistance(player.Body.Position) < _maxDistance*_maxDistance)
                return;

            ship.Vanish();
        }

        public void UpdateView(IShip ship, float elapsedTime) {}
        public void Dispose() {}

        public IEngineModification EngineModification { get { return null; } }
        public IFeaturesModification FeaturesModification { get { return null; } }
        public ISystemsModification SystemsModification { get { return null; } }
        public IStatsModification StatsModification { get { return null; } }
        public IUnitAction UnitAction { get { return null; } }

        private readonly float _maxDistance;
        private readonly IScene _scene;
    }
}
