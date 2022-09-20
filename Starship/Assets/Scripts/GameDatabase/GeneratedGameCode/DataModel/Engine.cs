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
	public partial class Engine
	{
		partial void OnDataDeserialized(EngineSerializable serializable, Database.Loader loader);

		public static Engine Create(EngineSerializable serializable, Database.Loader loader)
		{
			return new Engine(serializable, loader);
		}

		private Engine(EngineSerializable serializable, Database.Loader loader)
		{
			Position = serializable.Position;
			Size = UnityEngine.Mathf.Clamp(serializable.Size, 0f, 3.402823E+38f);

			OnDataDeserialized(serializable, loader);
		}

		public UnityEngine.Vector2 Position { get; private set; }
		public float Size { get; private set; }

		public static Engine DefaultValue { get; private set; }
	}
}
