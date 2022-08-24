//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System.Linq;
using GameDatabase.Enums;
using GameDatabase.Serializable;
using GameDatabase.Model;

namespace GameDatabase.DataModel
{
	public partial class Component
	{
		partial void OnDataDeserialized(ComponentSerializable serializable, Database.Loader loader);

		public static Component Create(ComponentSerializable serializable, Database.Loader loader)
		{
			return new Component(serializable, loader);
		}

		private Component(ComponentSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Component>(serializable.Id);
			loader.AddComponent(serializable.Id, this);

			Name = serializable.Name;
			Description = serializable.Description;
			DisplayCategory = serializable.DisplayCategory;
			Availability = serializable.Availability;
			Stats = loader.GetComponentStats(new ItemId<ComponentStats>(serializable.ComponentStatsId));
			if (Stats == null)
			    throw new DatabaseException(this.GetType().Name + ".Stats cannot be null - " + serializable.ComponentStatsId);
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));
			Level = UnityEngine.Mathf.Clamp(serializable.Level, 0, 1000);
			Icon = new SpriteId(serializable.Icon, SpriteId.Type.Component);
			Color = new ColorData(serializable.Color);
			Layout = new Layout(serializable.Layout);
			_cellType = serializable.CellType;
			Device = loader.GetDevice(new ItemId<Device>(serializable.DeviceId));
			Weapon = loader.GetWeapon(new ItemId<Weapon>(serializable.WeaponId));
			Ammunition = loader.GetAmmunition(new ItemId<Ammunition>(serializable.AmmunitionId));
			AmmunitionObsolete = loader.GetAmmunitionObsolete(new ItemId<AmmunitionObsolete>(serializable.AmmunitionId));
			_weaponSlotType = serializable.WeaponSlotType;
			DroneBay = loader.GetDroneBay(new ItemId<DroneBay>(serializable.DroneBayId));
			Drone = loader.GetShipBuild(new ItemId<ShipBuild>(serializable.DroneId));
			Restrictions = ComponentRestrictions.Create(serializable.Restrictions, loader);
			PossibleModifications = new ImmutableCollection<ComponentMod>(serializable.PossibleModifications?.Select(item => loader.GetComponentMod(new ItemId<ComponentMod>(item), true)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Component> Id;

		public string Name { get; private set; }
		public string Description { get; private set; }
		public ComponentCategory DisplayCategory { get; private set; }
		public Availability Availability { get; private set; }
		public ComponentStats Stats { get; private set; }
		public Faction Faction { get; private set; }
		public int Level { get; private set; }
		public SpriteId Icon { get; private set; }
		public ColorData Color { get; private set; }
		public Layout Layout { get; private set; }
		private readonly string _cellType;
		public Device Device { get; private set; }
		public Weapon Weapon { get; private set; }
		public Ammunition Ammunition { get; private set; }
		public AmmunitionObsolete AmmunitionObsolete { get; private set; }
		private readonly string _weaponSlotType;
		public DroneBay DroneBay { get; private set; }
		public ShipBuild Drone { get; private set; }
		public ComponentRestrictions Restrictions { get; private set; }
		public ImmutableCollection<ComponentMod> PossibleModifications { get; private set; }

		public static Component DefaultValue { get; private set; }
	}
}
