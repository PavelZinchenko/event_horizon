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
	public partial class DatabaseSettings
	{
		partial void OnDataDeserialized(DatabaseSettingsSerializable serializable, Database.Loader loader);

		public static DatabaseSettings Create(DatabaseSettingsSerializable serializable, Database.Loader loader)
		{
			return new DatabaseSettings(serializable, loader);
		}

		private DatabaseSettings(DatabaseSettingsSerializable serializable, Database.Loader loader)
		{
			DatabaseVersion = UnityEngine.Mathf.Clamp(serializable.DatabaseVersion, 1, 2147483647);
			ModName = serializable.ModName;
			ModId = serializable.ModId;
			ModVersion = UnityEngine.Mathf.Clamp(serializable.ModVersion, -2147483648, 2147483647);
			UnloadOriginalDatabase = serializable.UnloadOriginalDatabase;

			OnDataDeserialized(serializable, loader);
		}

		public int DatabaseVersion { get; private set; }
		public string ModName { get; private set; }
		public string ModId { get; private set; }
		public int ModVersion { get; private set; }
		public bool UnloadOriginalDatabase { get; private set; }

		public static DatabaseSettings DefaultValue { get; private set; }
	}
}
