//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace GameDatabase.Serializable
{
	[Serializable]
	public class DatabaseSettingsSerializable : SerializableItem
	{
		public int DatabaseVersion;
		public string ModName;
		public string ModId;
		public int ModVersion;
		public bool UnloadOriginalDatabase;
	}
}
