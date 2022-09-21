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
	public partial class BulletPrefab
	{
		partial void OnDataDeserialized(BulletPrefabSerializable serializable, Database.Loader loader);

		public static BulletPrefab Create(BulletPrefabSerializable serializable, Database.Loader loader)
		{
			return new BulletPrefab(serializable, loader);
		}

		private BulletPrefab(BulletPrefabSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<BulletPrefab>(serializable.Id);
			loader.AddBulletPrefab(serializable.Id, this);

			Shape = serializable.Shape;
			Image = new SpriteId(serializable.Image, SpriteId.Type.Ammunition);
			Size = UnityEngine.Mathf.Clamp(serializable.Size, 0.01f, 100f);
			Margins = UnityEngine.Mathf.Clamp(serializable.Margins, 0f, 1f);
			MainColor = new ColorData(serializable.MainColor);
			MainColorMode = serializable.MainColorMode;
			SecondColor = new ColorData(serializable.SecondColor);
			SecondColorMode = serializable.SecondColorMode;

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<BulletPrefab> Id;

		public BulletShape Shape { get; private set; }
		public SpriteId Image { get; private set; }
		public float Size { get; private set; }
		public float Margins { get; private set; }
		public ColorData MainColor { get; private set; }
		public ColorMode MainColorMode { get; private set; }
		public ColorData SecondColor { get; private set; }
		public ColorMode SecondColorMode { get; private set; }

		public static BulletPrefab DefaultValue { get; private set; }
	}
}
