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
	public class ComponentSerializable : SerializableItem
	{
		public string Name;
		public string Description;
		public ComponentCategory DisplayCategory;
		public Availability Availability;
		public int ComponentStatsId;
		public int Faction;
		public int Level;
		public string Icon;
		public string Color;
		public string Layout;
		public string CellType;
		public int DeviceId;
		public int WeaponId;
		public int AmmunitionId;
		public string WeaponSlotType;
		public int DroneBayId;
		public int DroneId;
		public ComponentRestrictionsSerializable Restrictions;
		public int[] PossibleModifications;
	}
}
