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
	public class ShipBuildSerializable : SerializableItem
	{
		public int ShipId;
		public bool NotAvailableInGame;
		public DifficultyClass DifficultyClass;
		public int BuildFaction;
		public InstalledComponentSerializable[] Components;
	}
}
