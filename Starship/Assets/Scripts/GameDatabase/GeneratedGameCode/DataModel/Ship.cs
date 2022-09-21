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
	public partial class Ship
	{
		partial void OnDataDeserialized(ShipSerializable serializable, Database.Loader loader);

		public static Ship Create(ShipSerializable serializable, Database.Loader loader)
		{
			return new Ship(serializable, loader);
		}

		private Ship(ShipSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Ship>(serializable.Id);
			loader.AddShip(serializable.Id, this);

			ShipCategory = serializable.ShipCategory;
			Name = serializable.Name;
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));
			SizeClass = serializable.SizeClass;
			IconImage = new SpriteId(serializable.IconImage, SpriteId.Type.ShipIcon);
			IconScale = UnityEngine.Mathf.Clamp(serializable.IconScale, 0.1f, 100f);
			ModelImage = new SpriteId(serializable.ModelImage, SpriteId.Type.Ship);
			ModelScale = UnityEngine.Mathf.Clamp(serializable.ModelScale, 0.1f, 100f);
			_enginePosition = serializable.EnginePosition;
			EngineColor = new ColorData(serializable.EngineColor);
			_engineSize = UnityEngine.Mathf.Clamp(serializable.EngineSize, 0f, 1f);
			Engines = new ImmutableCollection<Engine>(serializable.Engines?.Select(item => Engine.Create(item, loader)));
			EnergyResistance = UnityEngine.Mathf.Clamp(serializable.EnergyResistance, -3.402823E+38f, 3.402823E+38f);
			KineticResistance = UnityEngine.Mathf.Clamp(serializable.KineticResistance, -3.402823E+38f, 3.402823E+38f);
			HeatResistance = UnityEngine.Mathf.Clamp(serializable.HeatResistance, -3.402823E+38f, 3.402823E+38f);
			Regeneration = serializable.Regeneration;
			BaseWeightModifier = UnityEngine.Mathf.Clamp(serializable.BaseWeightModifier, -0.99f, 3.402823E+38f);
			BuiltinDevices = new ImmutableCollection<Device>(serializable.BuiltinDevices?.Select(item => loader.GetDevice(new ItemId<Device>(item), true)));
			Layout = new Layout(serializable.Layout);
			Barrels = new ImmutableCollection<Barrel>(serializable.Barrels?.Select(item => Barrel.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Ship> Id;

		public ShipCategory ShipCategory { get; private set; }
		public string Name { get; private set; }
		public Faction Faction { get; private set; }
		public SizeClass SizeClass { get; private set; }
		public SpriteId IconImage { get; private set; }
		public float IconScale { get; private set; }
		public SpriteId ModelImage { get; private set; }
		public float ModelScale { get; private set; }
		private readonly UnityEngine.Vector2 _enginePosition;
		public ColorData EngineColor { get; private set; }
		private readonly float _engineSize;
		public ImmutableCollection<Engine> Engines { get; private set; }
		public float EnergyResistance { get; private set; }
		public float KineticResistance { get; private set; }
		public float HeatResistance { get; private set; }
		public bool Regeneration { get; private set; }
		public float BaseWeightModifier { get; private set; }
		public ImmutableCollection<Device> BuiltinDevices { get; private set; }
		public Layout Layout { get; private set; }
		public ImmutableCollection<Barrel> Barrels { get; private set; }

		public static Ship DefaultValue { get; private set; }
	}
}
