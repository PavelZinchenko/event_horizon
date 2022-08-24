using System;
using System.Linq;
using Constructor.Satellites;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Maths;
using Utils;

namespace Constructor.Ships
{
    public abstract class BaseShip : IShip
    {
        protected BaseShip(IShipModel model)
        {
            _model = model ?? throw new System.ArgumentException("ShipWrapper is null");
            _experience = 0;
        }

        public ItemId<Ship> Id => _model.Id;

        public virtual string Name { get => _model.OriginalName; set => throw new InvalidOperationException(); }

        public IShipModel Model => _model;
        public ShipColorScheme ColorScheme => _colorScheme;

        public abstract IItemCollection<IntegratedComponent> Components { get; }

        public ISatellite FirstSatellite
        {
            get => _firstSatellite;
            set
            {
                _firstSatellite = value;
                DataChanged = true;
            }
        }

        public ISatellite SecondSatellite
        {
            get => _secondSatellite;
            set
            {
                _secondSatellite = value;
                DataChanged = true;
            }
        }

        public virtual DifficultyClass ExtraThreatLevel => DifficultyClass.Default;

        public Experience Experience
        {
            get => _experience;
            set
            {
                _experience = value;
                DataChanged = true;
            }
        }

        public virtual ShipBuilder CreateBuilder()
        {
            var builder = new ShipBuilder(this);
            var scale = Experience.PowerMultiplier;
            builder.Bonuses.ArmorPointsMultiplier *= scale;
            builder.Bonuses.ShieldPointsMultiplier *= scale;
            builder.Bonuses.DamageMultiplier *= scale;
            builder.Bonuses.RammingDamageMultiplier *= scale;

            if (FirstSatellite != null)
                builder.AddSatellite(FirstSatellite, CompanionLocation.Left);
            if (SecondSatellite != null)
                builder.AddSatellite(SecondSatellite, CompanionLocation.Right);

            return builder;
        }

        public bool DataChanged
        {
            get
            {
                if (_dataChanged)
                    return true;
                if (FirstSatellite != null && FirstSatellite.DataChanged)
                    return true;
                if (SecondSatellite != null && SecondSatellite.DataChanged)
                    return true;
                if (_model.DataChanged)
                    return true;
                if (_colorScheme.IsChanged)
                    return true;

                return false;
            }
            set
            {
                _dataChanged = value;
                if (_dataChanged)
                    return;

                if (FirstSatellite != null)
                    FirstSatellite.DataChanged = false;
                if (SecondSatellite != null)
                    SecondSatellite.DataChanged = false;

                _model.DataChanged = false;
                _colorScheme.IsChanged = false;
            }
        }

        public int RemoveInvalidComponents(IGameItemCollection<ComponentInfo> inventory)
        {
            var layout = new ShipLayout(Model.Layout, Model.Barrels, Enumerable.Empty<IntegratedComponent>());
            var index = 0;
            var components = Components;
            var count = 0;

            while (index < components.Count)
            {
                var component = components[index];
                if (layout.InstallComponent(component.Info, component.X, component.Y) >= 0)
                {
                    index++;
                    continue;
                }

                components.RemoveAt(index);
                inventory.Add(component.Info);
                count++;
            }

            return count;
        }

        private Experience _experience;
        private ISatellite _firstSatellite;
        private ISatellite _secondSatellite;
        private bool _dataChanged;
        private readonly ShipColorScheme _colorScheme = new ShipColorScheme();
        private readonly IShipModel _model;
    }
}
