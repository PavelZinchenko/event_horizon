using UnityEngine;
using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace Constructor
{
	public interface IShipSpecification
	{
	    ShipType Type { get; }
	    Model.IShipStats Stats { get; }
		IEnumerable<IWeaponPlatformData> Platforms { get; }
		IEnumerable<IDeviceData> Devices { get; }
		IEnumerable<IDroneBayData> DroneBays { get; }

		IShipSpecification CopyWithStats(Model.IShipStats stats);
	}

	public struct ShipType
    {
        public ShipType(ItemId<Ship> id, DifficultyClass shipClass, int level, int size)
        {
            Id = id;
            Class = shipClass;
            Size = size;
            Level = level;
        }

        public readonly ItemId<Ship> Id;
        public readonly DifficultyClass Class;
        public readonly int Size;
        public readonly int Level;
    }

	public interface IWeaponPlatformData
	{
		Vector2 Position { get; }
		float Rotation { get; }
		float Offset { get; }
        float Size { get; }
        SpriteId Image { get; }
	    float AutoAimingArc { get; }
	    float RotationSpeed { get; }
        ICompanionData Companion { get; }
	    IEnumerable<IWeaponData> Weapons { get; }
	    IEnumerable<IWeaponDataObsolete> WeaponsObsolete { get; }
    }

    public interface ICompanionData
	{
		Satellite Satellite { get; }
		CompanionLocation Location { get; }
		float Weight { get; }
	}

    public interface IWeaponData
    {
        Weapon Weapon { get; }
        Ammunition Ammunition { get; }
        WeaponStatModifier Stats { get; }
        int KeyBinding { get; }
    }

    public struct WeaponStatModifier
    {
        public StatMultiplier DamageMultiplier;
        public StatMultiplier RangeMultiplier;
        public StatMultiplier LifetimeMultiplier;
        public StatMultiplier FireRateMultiplier;
        public StatMultiplier EnergyCostMultiplier;
        public StatMultiplier AoeRadiusMultiplier;
        public StatMultiplier VelocityMultiplier;
        public StatMultiplier WeightMultiplier;
        public Color? Color;
    }

    public interface IWeaponDataObsolete
	{
		WeaponStats Weapon { get; }
        AmmunitionObsoleteStats Ammunition { get; }
        int KeyBinding { get; }
	}
	
	public interface IDeviceData
	{
		DeviceStats Device { get; }
		int KeyBinding { get; }
	}
	
    public enum DroneBehaviour { Aggressive = 0, Defensive = 1 }

	public interface IDroneBayData
	{
		DroneBayStats DroneBay { get; }
		ShipBuild Drone { get; }
		int KeyBinding { get; }
        DroneBehaviour Behaviour { get; }
	}
}
