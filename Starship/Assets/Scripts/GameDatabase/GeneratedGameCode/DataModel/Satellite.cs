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
	public partial class Satellite
	{
		partial void OnDataDeserialized(SatelliteSerializable serializable, Database.Loader loader);

		public static Satellite Create(SatelliteSerializable serializable, Database.Loader loader)
		{
			return new Satellite(serializable, loader);
		}

		private Satellite(SatelliteSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Satellite>(serializable.Id);
			loader.AddSatellite(serializable.Id, this);

			Name = serializable.Name;
			ModelImage = new SpriteId(serializable.ModelImage, SpriteId.Type.Satellite);
			ModelScale = UnityEngine.Mathf.Clamp(serializable.ModelScale, 0.1f, 100f);
			SizeClass = serializable.SizeClass;
			Layout = new Layout(serializable.Layout);
			Barrels = new ImmutableCollection<Barrel>(serializable.Barrels?.Select(item => Barrel.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Satellite> Id;

		public string Name { get; private set; }
		public SpriteId ModelImage { get; private set; }
		public float ModelScale { get; private set; }
		public SizeClass SizeClass { get; private set; }
		public Layout Layout { get; private set; }
		public ImmutableCollection<Barrel> Barrels { get; private set; }

		public static Satellite DefaultValue { get; private set; }
	}
}
