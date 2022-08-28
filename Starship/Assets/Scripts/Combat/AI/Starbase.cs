using System.Collections.Generic;
using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Systems.DroneBays;
using Combat.Component.Systems.Weapons;
using Combat.Scene;
using Combat.Unit;

namespace Combat.Ai
{
    public class Starbase : IController
    {
        public Starbase(IScene scene, IShip ship, bool combatMode)
        {
            _ship = ship;
            _scene = scene;
            _targets = new TargetList(_scene);

            if (combatMode)
                CreateStrategy();
            else
                CreateIdleStrategy();
        }

        public bool IsAlive { get { return _ship.IsActive(); } }

        public void Update(float deltaTime)
        {
            var context = new Context(_ship, null, _targets, null, _currentTime);
            var controls = new ShipControls(_ship);

            _targets.Update(deltaTime, _ship, null);
            foreach (var policy in _strategy)
                policy.Perform(context, ref controls);

            controls.Apply(_ship);
            _currentTime += deltaTime;
        }

        private void CreateStrategy()
        {
            _strategy.Clear();

            var rechargingState = new State<bool>();

            for (var i = 0; i < _ship.Systems.All.Count; i++)
                if (_ship.Systems.All[i] is IDroneBay)
                    _strategy.Add(new Policy(new EnergyRechargedCondition(0.1f, 0.5f, rechargingState), new DroneAction(i)));

            _strategy.Add(new Policy(new EnergyRechargedCondition(0.1f, 0.5f, rechargingState), new CommonWeaponsAttackAction(_ship, false, true)));

            for (var i = 0; i < _ship.Systems.All.Count; i++)
            {
                var weapon = _ship.Systems.All.Weapon(i);
                if (weapon == null)
                    continue;

                if (weapon.Info.BulletEffectType == BulletEffectType.Special || weapon.Info.BulletEffectType == BulletEffectType.Repair)
                {
                    _strategy.Add(new Policy(new All(
                        new HasEnergyCondition(0.5f),
                        new EnergyRechargedCondition(0.1f, 0.5f, rechargingState)
                    ), new CommonWeaponsAttackAction(i, false, true)));
                }

                if (weapon.Info.WeaponType == WeaponType.RequiredCharging)
                    _strategy.Add(new Policy(new AlwaysTrueCondition(), new ChargedWeaponAction(i)));

                if (weapon.Info.WeaponType == WeaponType.Manageable)
                    _strategy.Add(new Policy(new AlwaysTrueCondition(), new ControlledWeaponAction(i, true)));
            }
        }

        private void CreateIdleStrategy()
        {
            _strategy.Clear();

            for (var i = 0; i < _ship.Systems.All.Count; i++)
                if (_ship.Systems.All[i] is IDroneBay)
                    _strategy.Add(new Policy(new AlwaysTrueCondition(), new DroneAction(i)));
        }

        private readonly List<Policy> _strategy = new List<Policy>();
        private float _currentTime;
        private readonly TargetList _targets;
        private readonly IShip _ship;
        private readonly IScene _scene;

        public class Factory : IControllerFactory
        {
            public Factory(IScene scene, bool combatMode)
            {
                _combatMode = combatMode;
                _scene = scene;
            }

            public IController Create(IShip ship)
            {
                return new Starbase(_scene, ship, _combatMode);
            }

            private readonly bool _combatMode;
            private readonly IScene _scene;
        }
    }
}
