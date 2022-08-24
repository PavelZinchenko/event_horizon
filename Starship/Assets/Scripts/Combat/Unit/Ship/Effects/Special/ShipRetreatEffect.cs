using Combat.Component.Engine;
using Combat.Component.Features;
using Combat.Component.Ship;
using Combat.Component.Ship.Effects;
using Combat.Component.Stats;
using Combat.Component.Systems;
using Combat.Component.Triggers;

namespace Combat.Unit.Ship.Effects.Special
{
    public class ShipRetreatEffect : IShipEffect, IEngineModification, ISystemsModification
    {
        public ShipRetreatEffect(float cooldown, params IUnitEffect[] effects)
        {
            _cooldown = cooldown;
            foreach (var item in effects)
                _triggers.Add(item);

            _triggers.Invoke(ConditionType.OnActivate);
        }

        public bool IsAlive { get { return _elapsedTime < _cooldown; } }

        public bool CanActivateSystem(ISystem system) { return false; }
        public void OnSystemActivated(ISystem system) {}

        public bool TryApplyModification(ref EngineData data)
        {
            if (!IsAlive)
                return false;

            data.Throttle = 0;
            data.HasCourse = false;
            data.TurnRate = 0;
            data.Propulsion = 0;
            return true;
        }

        public void UpdatePhysics(IShip ship, float elapsedTime)
        {
            if (!IsAlive)
                return;

            _elapsedTime += elapsedTime;

            if (_elapsedTime < _cooldown)
                _triggers.UpdatePhysics(elapsedTime);
            else
            {
                _triggers.Invoke(ConditionType.OnDeactivate);
                ship.Vanish();
            }
        }

        public void UpdateView(IShip ship, float elapsedTime)
        {
            _triggers.UpdateView(elapsedTime);
        }

        public void Dispose()
        {
            _triggers.Dispose();
        }

        public IEngineModification EngineModification { get { return this; } }
        public IFeaturesModification FeaturesModification { get { return null; } }
        public ISystemsModification SystemsModification { get { return this; } }
        public IStatsModification StatsModification { get { return null; } }
        public IUnitAction UnitAction { get { return null; } }

        private float _elapsedTime;
        private readonly float _cooldown;
        private readonly UnitTriggers _triggers = new UnitTriggers();
    }
}
