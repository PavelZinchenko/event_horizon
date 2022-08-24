using System.Collections.Generic;
using Constructor.Model;
using Constructor.Modification;
using Constructor.Ships;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace Constructor.Component
{
	public interface IWeaponPlatformStats
	{
	    void ChangePlatformType(PlatformType type);
	}

    public interface IComponent
	{
		void UpdateStats(ref ShipEquipmentStats stats);
		void UpdateWeaponPlatform(IWeaponPlatformStats stats);
	    IEnumerable<WeaponData> Weapons { get; }
	    IEnumerable<KeyValuePair<WeaponStats, AmmunitionObsoleteStats>> WeaponsObsolete { get; }
		IEnumerable<KeyValuePair<DroneBayStats, ShipBuild>> DroneBays { get; }
        IEnumerable<DeviceStats> Devices { get; }
        ActivationType ActivationType { get; }
		bool IsSuitable(IShipModel ship);

	    int UpgradeLevel { get; set; }
	    IModification Modification { get; set; }
	    IEnumerable<ComponentModType> SuitableModifications { get; }
	}

    public struct WeaponData
    {
        public Weapon Weapon;
        public Ammunition Ammunition;
        public WeaponStatModifier StatModifier;
    }
}
