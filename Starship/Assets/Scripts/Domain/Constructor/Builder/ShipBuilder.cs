using System;
using System.Collections.Generic;
using System.Linq;
using Constructor.Component;
using Constructor.Detail;
using Constructor.Model;
using Constructor.Satellites;
using Constructor.Ships;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine;

namespace Constructor
{
	public class ShipBuilder
	{
		public ShipBuilder(ShipBuild ship)
            : this(new ShipModel(ship.Ship, ship.BuildFaction), ship.Components.Select<InstalledComponent, IntegratedComponent>(ComponentExtensions.FromDatabase))
		{
            var boost = 1f + 0.5f*(int)ship.DifficultyClass;
			Bonuses.DamageMultiplier *= boost;
			Bonuses.ArmorPointsMultiplier *= boost;
            Bonuses.ShieldPointsMultiplier *= boost;
		    //Bonuses.RammingDamageMultiplier *= boost;
            _shipClass = ship.DifficultyClass;
		}

	    public ShipBuilder(IShipModel model, IEnumerable<IntegratedComponent> components)
	    {
	        _ship = model;
	        _shipComponents = new List<IntegratedComponent>(components);
	    }

        public ShipBuilder(IShip ship)
            : this(ship.Model, ship.Components)
		{
		    _shipLevel = ship.Experience.Level;
		    _shipClass = ship.ExtraThreatLevel;
            TurretColor = ShipColor = new ColorScheme(ship.ColorScheme.Color, ship.ColorScheme.Hue, ship.ColorScheme.Saturation);
		}

		public ShipBonuses Bonuses;
	    public ColorScheme ShipColor;
	    public ColorScheme TurretColor;

        //public List<IntegratedComponent> ShipComponents { get { return _shipComponents; } }

        public IComponentConverter Converter { get { return _converter; } set { _converter = value ?? DefaultComponentConverter.Instance; } }

		public void AddSatellite(ISatellite spec, CompanionLocation location)
		{
			_satellites.Add(new KeyValuePair<ISatellite, CompanionLocation>(spec, location));
		}

		public IShipSpecification Build(ShipSettings settings)
		{
			var size = Size;
		    var data = new ShipBuilderResult();

		    var stats = new ShipStatsCalculator(_ship.OriginalShip, settings);
		    stats.Bonuses = Bonuses;
		    stats.ShipColor = ShipColor;
		    stats.TurretColor = TurretColor;
		    stats.BaseStats = _ship.Stats;

		    data.Stats = stats;
			data.Type = new ShipType(_ship.Id, _shipClass, _shipLevel, size);
			data._platforms.AddRange(GetPlatforms(stats, settings));

		    foreach (var item in stats.BuiltinDevices)
		        data._devices.Add(new DeviceData(item.Stats, -1));

		    var uniqueComponents = new HashSet<string>();

			foreach (var item in Components)
			{
			    var uniqueKey = item.Info.Data.GetUniqueKey();
                if (!string.IsNullOrEmpty(uniqueKey))
					if (!uniqueComponents.Add(uniqueKey))
						continue;

			    var component = item.Info.CreateComponent(_ship.Layout.CellCount);
				if (component == null || !component.IsSuitable(_ship))
					continue;

                component.UpdateStats(ref stats.EquipmentStats);

                foreach (var spec in component.Devices)
                    data._devices.Add(new DeviceData(spec, item.KeyBinding));
                foreach (var spec in component.DroneBays)
                    data._droneBays.Add(new DroneBayData(spec.Key, spec.Value, item.KeyBinding, (DroneBehaviour)item.Behaviour));

                if (item.BarrelId < 0)
                    continue;
				var platform = data._platforms.Find(obj => obj.BarrelId == item.BarrelId);
                if (platform == null)
                    continue;

                component.UpdateWeaponPlatform(platform);
			    foreach (var spec in component.Weapons)
			        platform.AddWeapon(spec.Weapon, spec.Ammunition, spec.StatModifier, item.KeyBinding);
                foreach (var spec in component.WeaponsObsolete)
                    platform.AddWeapon(spec.Key, spec.Value, item.KeyBinding);
			}

			foreach (var platform in data._platforms)
			{
				platform.DamageMultiplier = stats.WeaponDamageMultiplier.Value;
				platform.FireRateMultiplier = stats.WeaponFireRateMultiplier.Value;
				platform.EnergyConsumptionMultiplier = stats.WeaponEnergyCostMultiplier.Value;
				platform.RangeMultiplier = stats.WeaponRangeMultiplier.Value;
			}

			foreach (var droneBay in data._droneBays)
			{
			    droneBay.TotalDamageMultiplier = stats.Bonuses.DamageMultiplier.Value;
			    droneBay.TotalDefenseMultiplier = stats.Bonuses.ArmorPointsMultiplier.Value;

                droneBay.DroneDamageModifier = stats.DroneDamageMultiplier.Bonus;
                droneBay.DroneDefenseModifier = stats.DroneDefenseMultiplier.Bonus;
                droneBay.DroneSpeedModifier = stats.DroneSpeedMultiplier.Bonus;
				droneBay.DroneRangeModifier = stats.DroneRangeMultiplier.Bonus;
			}

			return data;
		}

		private int Size
		{
			get
			{
				var size = 0;
				foreach (var item in _satellites)
					size += item.Key.Information.Layout.CellCount;

				return _ship.Layout.CellCount + size/2;
			}
		}

		private IEnumerable<WeaponPlatform> GetPlatforms(IShipStats stats, ShipSettings settings)
		{
			var id = 0;
		    foreach (var barrel in _ship.Barrels)
		    {
		        var platform = new WeaponPlatform(barrel) { BarrelId = id++ };
                if (stats.TargetingSystem) platform.ChangePlatformType(PlatformType.TargetingUnit);
		        yield return platform;
		    }

            foreach (var item in _satellites)
			{
				var barrels = item.Key.Information.Barrels;
				var count = barrels.Count();

                if (count > 1)
                    throw new ArgumentException("companions should have one barrel");

                if (count == 0)
                    yield return new WeaponPlatform(Barrel.Empty) { Companion = new CompanionSpec(item.Key.Information, item.Value, settings) };
                else
                    yield return new WeaponPlatform(barrels.First()) { Companion = new CompanionSpec(item.Key.Information, item.Value, settings), BarrelId = id++ };
            }
		}

		private IEnumerable<ComponentSpec> Components
		{
			get
			{
				int barrel = 0;
			    foreach (var component in _shipComponents)
			        yield return _converter.Process(component, barrel);

			    barrel += _ship.Barrels.Count;

				foreach (var item in _satellites)
				{
					foreach (var component in item.Key.Components)
                        yield return _converter.Process(component, barrel);

                    barrel += item.Key.Information.Barrels.Count;
				}
			}
		}

        private IComponentConverter _converter = DefaultComponentConverter.Instance;
	    private readonly int _shipLevel;
        private readonly IShipModel _ship;
	    private readonly DifficultyClass _shipClass = DifficultyClass.Default;
		private readonly List<IntegratedComponent> _shipComponents;
		private readonly List<KeyValuePair<ISatellite, CompanionLocation>> _satellites = new List<KeyValuePair<ISatellite, CompanionLocation>>();
	}

	public class DeviceData : IDeviceData
	{
		public DeviceData(DeviceStats spec, int key)
		{
			Device = spec;
			KeyBinding = spec.ActivationType.ValidateKey(key);
		}

		public DeviceStats Device { get; private set; }
		public int KeyBinding { get; private set; }
	}

	public class DroneBayData : IDroneBayData
	{
		public DroneBayData(DroneBayStats spec, ShipBuild drone, int key, DroneBehaviour behaviour)
		{
			_droneBay = spec;
			Drone = drone;
			KeyBinding = spec.ActivationType.ValidateKey(key);
		    Behaviour = behaviour;
		}

		public DroneBayStats DroneBay
		{
			get
			{
				var stats = _droneBay; 
				stats.DamageMultiplier += DroneDamageModifier;
                stats.DefenseMultiplier += DroneDefenseModifier;
                stats.SpeedMultiplier += DroneSpeedModifier;
                stats.Range *= 1.0f + DroneRangeModifier;

			    stats.DamageMultiplier *= TotalDamageMultiplier;
			    stats.DefenseMultiplier *= TotalDefenseMultiplier;

				return stats;
			}
		}
		public ShipBuild Drone { get; private set; }
		public int KeyBinding { get; private set; }
	    public DroneBehaviour Behaviour { get; private set; }

        public float TotalDamageMultiplier { get; set; }
        public float TotalDefenseMultiplier { get; set; }

        public float DroneDamageModifier { get; set; }
        public float DroneDefenseModifier { get; set; }
        public float DroneSpeedModifier { get; set; }
		public float DroneRangeModifier { get; set; }

        private readonly DroneBayStats _droneBay;
	}

	namespace Detail
	{
		public class WeaponPlatform : IWeaponPlatformData, IWeaponPlatformStats
		{
			public WeaponPlatform(Barrel barrel)
			{
				Position = barrel.Position;
				Rotation = barrel.Rotation;
				Offset = barrel.Offset;

			    AutoAimingArc = barrel.AutoAimingArc > 0 ? barrel.AutoAimingArc : PlatformTypeToAngle(barrel.PlatformType);
			    RotationSpeed = barrel.RotationSpeed;
                DamageMultiplier = 1;
				FireRateMultiplier = 1;
				EnergyConsumptionMultiplier = 1;
			    BarrelId = -1;
			    Size = barrel.Size;
			    Image = barrel.Image;
			}

		    public void ChangePlatformType(PlatformType type)
		    {
		        AutoAimingArc = Mathf.Max(AutoAimingArc, PlatformTypeToAngle(type));
		    }

		    private static float PlatformTypeToAngle(PlatformType type)
		    {
		        switch (type)
		        {
		            case PlatformType.AutoTarget:
		                return 360;
		            case PlatformType.AutoTargetFrontal:
		                return 80;
		            case PlatformType.TargetingUnit:
		                return 20;
		            case PlatformType.Common:
		            default:
		                return 0;
		        }
		    }

            public Vector2 Position { get; }
			public float Rotation { get; }
			public float Offset { get; }
            public float Size { get; }
            public SpriteId Image { get; }
		    public float AutoAimingArc { get; private set; }
		    public float RotationSpeed { get; private set; }
			public ICompanionData Companion { get; set; }

		    public int BarrelId { get; set; }
		    public float DamageMultiplier { get; set; }
			public float FireRateMultiplier { get; set; }
			public float EnergyConsumptionMultiplier { get; set; }
			public float RangeMultiplier { get; set; }

		    public IEnumerable<IWeaponData> Weapons
		    {
		        get
		        {
		            foreach (var item in _weapons)
		            {
		                item.DamageMultiplier = DamageMultiplier;
		                item.FireRateMultiplier = FireRateMultiplier;
		                item.EnergyConsumptionMultiplier = EnergyConsumptionMultiplier;
		                item.RangeMultiplier = RangeMultiplier;
		                yield return item;
		            }
		        }
            }

		    public IEnumerable<IWeaponDataObsolete> WeaponsObsolete
			{
				get
				{
					foreach (var item in _weaponsObsolete)
					{
						item.DamageMultiplier = DamageMultiplier;
						item.FireRateMultiplier = FireRateMultiplier;
						item.EnergyConsumptionMultiplier = EnergyConsumptionMultiplier;
						item.RangeMultiplier = RangeMultiplier;
						yield return item;
					}
				}
			}
			
			public void AddWeapon(WeaponStats weapon, AmmunitionObsoleteStats ammunition, int key)
			{
				_weaponsObsolete.Add(new WeaponDataObsolete(weapon, ammunition, key));
			}

		    public void AddWeapon(Weapon weapon, Ammunition ammunition, WeaponStatModifier stats, int key)
		    {
		        _weapons.Add(new WeaponData(weapon, ammunition, stats, key));
		    }

            private readonly List<WeaponData> _weapons = new List<WeaponData>();
	        private readonly List<WeaponDataObsolete> _weaponsObsolete = new List<WeaponDataObsolete>();

		    public class WeaponData : IWeaponData
		    {
		        public WeaponData(Weapon weapon, Ammunition ammunition, WeaponStatModifier stats, int key)
		        {
		            _weapon = weapon;
		            _ammunition = ammunition;
		            _stats = stats;
		            KeyBinding = weapon.Stats.ActivationType.ValidateKey(key);
		        }

		        public Weapon Weapon { get { return _weapon; } }
		        public Ammunition Ammunition { get { return _ammunition; } }

		        public WeaponStatModifier Stats
		        {
		            get
		            {
		                var stats = _stats;
		                stats.DamageMultiplier *= DamageMultiplier;
		                stats.EnergyCostMultiplier *= EnergyConsumptionMultiplier;
		                stats.FireRateMultiplier *= FireRateMultiplier;
		                stats.RangeMultiplier *= RangeMultiplier;
		                return stats;
		            }
		        }

		        public int KeyBinding { get; private set; }

		        public float DamageMultiplier { get; set; }
		        public float FireRateMultiplier { get; set; }
		        public float EnergyConsumptionMultiplier { get; set; }
		        public float RangeMultiplier { get; set; }

		        private readonly Weapon _weapon;
		        private readonly Ammunition _ammunition;
		        private readonly WeaponStatModifier _stats;
		    }

            public class WeaponDataObsolete : IWeaponDataObsolete
			{
				public WeaponDataObsolete(WeaponStats weapon, AmmunitionObsoleteStats ammunition, int key)
				{
					_weapon = weapon;
				    _ammunition = ammunition;
					KeyBinding = weapon.ActivationType.ValidateKey(key);
				}

			    public WeaponStats Weapon
			    {
			        get
			        {
			            var stats = _weapon;
                        stats.FireRate *= FireRateMultiplier;
			            return stats;
			        }
                }

                public AmmunitionObsoleteStats Ammunition 
				{
					get
					{
						var stats = _ammunition;
						stats.Damage *= DamageMultiplier;
						stats.EnergyCost *= EnergyConsumptionMultiplier;
						stats.Range *= RangeMultiplier;
						return stats;
					}
				}
				
				public int KeyBinding { get; private set; }
				
				public float DamageMultiplier { get; set; }
				public float FireRateMultiplier { get; set; }
				public float EnergyConsumptionMultiplier { get; set; }
				public float RangeMultiplier { get; set; }
				
				private readonly WeaponStats _weapon;
                private readonly AmmunitionObsoleteStats _ammunition;
            }
		}
		
		public class ShipBuilderResult : IShipSpecification
		{
		    public ShipType Type { get; set; }
			public IShipStats Stats { get; set; }
            public IEnumerable<IWeaponPlatformData> Platforms { get { return _platforms.Cast<IWeaponPlatformData>(); } }
			public IEnumerable<IDeviceData> Devices { get { return _devices.Cast<IDeviceData>(); } }
			public IEnumerable<IDroneBayData> DroneBays { get { return _droneBays.Cast<IDroneBayData>(); } }
			
			public readonly List<WeaponPlatform> _platforms = new List<WeaponPlatform>();
			public readonly List<DeviceData> _devices = new List<DeviceData>();
			public readonly List<DroneBayData> _droneBays = new List<DroneBayData>();
		}
		
		public class CompanionSpec : ICompanionData
		{
			public CompanionSpec(Satellite satellite, CompanionLocation location, ShipSettings settings)
			{
				Satellite = satellite;
				Location = location;
				Weight = Satellite.Layout.CellCount * settings.DefaultWeightPerCell / 1000f;
			}
			
			public Satellite Satellite { get; private set; }
			public CompanionLocation Location { get; private set; }
			public float Weight { get; private set; }
		}		
	}	
}
