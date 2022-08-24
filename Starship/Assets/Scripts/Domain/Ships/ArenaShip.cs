using System;
using System.Collections.Generic;
using Constructor.Satellites;
using Utils;

namespace Constructor.Ships
{
    public class ArenaShip : BaseShip
    {
        public ArenaShip(IShip ship, float powerMultiplier = 1.0f)
            : base(ship.Model.Clone())
        {
            _name = ship.Name;
            _powerMultiplier = powerMultiplier;
            _components.Assign(ship.Components);
            FirstSatellite = ship.FirstSatellite.CreateCopy();
            SecondSatellite = ship.SecondSatellite.CreateCopy();
        }

        public override string Name
        {
            get { return _name; }
            set { base.Name = value; }
        }

        public override ShipBuilder CreateBuilder()
        {
            var builder = base.CreateBuilder();

            builder.Bonuses.ArmorPointsMultiplier *= _powerMultiplier;
            builder.Bonuses.ShieldPointsMultiplier *= _powerMultiplier;
            builder.Bonuses.DamageMultiplier *= _powerMultiplier;
            builder.Bonuses.RammingDamageMultiplier *= _powerMultiplier;

            return builder;
        }

        public override IItemCollection<IntegratedComponent> Components { get { return _components; } }

        private readonly ObservableCollection<IntegratedComponent> _components = new ObservableCollection<IntegratedComponent>();
        private readonly string _name;
        private readonly float _powerMultiplier;
    }
}
