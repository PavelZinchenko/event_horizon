using Combat.Ai.Condition;
using Combat.Component.Ship;
using Combat.Component.Systems.Devices;

namespace Combat.Ai
{
    public static class DefenseRules
    {
        public static void AvoidThreats(this StrategyBase strategy)
        {
            strategy.AddPolicy(new HasThreatsCondition(5.0f), new AvoidThreatsAction());
        }

        public static void UseDefenseSystems(this StrategyBase strategy, IShip ship, int level)
        {
            if (level > 60)
            {
                for (var i = 0; i < ship.Systems.All.Count; i++)
                {
                    var weapon = ship.Systems.All.Weapon(i);
                    if (weapon == null)
                        continue;

                    if (weapon.Info.Recoil > 3f)
                    {
                        strategy.AddPolicy(
                            new All(
                                new AwakeCondition(level),
                                new HasEnergyCondition(0.5f),
                                new HasThreatsCondition(0.8f)
                                ), new RecoilAction(i));
                    }
                }
            }

            var phaseShiftCooldown = new State<float>(-1f);

            for (var i = 0; i < ship.Systems.All.Count; i++)
            {
                var device = ship.Systems.All.Device(i);
                if (device == null)
                    continue;

                if (device is FrontalShieldDevice)
                    strategy.AddPolicy(
                        new All(
                        new HasEnergyCondition(0.5f),
                        new HasThreatsCondition(0.5f)
                        ), new ShieldAction(i));

                if (level > 50)
                {
                    if (device is GhostDevice)
                    {
                        strategy.AddPolicy(new LessTimePassedCondition(phaseShiftCooldown, 1.0f), new ActivateDeviceAction(i));
                        strategy.AddPolicy(new All(new AwakeCondition(level), new HasEnergyCondition(0.5f), new HasThreatsCondition(0.4f)), new ActivateDeviceAction(i, phaseShiftCooldown));
                    }

                    if (device is TeleporterDevice || device is PointDefenseSystem || device is EnergyShieldDevice || device is FortificationDevice)
                        strategy.AddPolicy(
                            new All(
                            new AwakeCondition(level),
                            new HasEnergyCondition(0.5f),
                            new HasThreatsCondition(0.4f)
                            ), new ActivateDeviceAction(i));
                }
            }
        }
    }
}
