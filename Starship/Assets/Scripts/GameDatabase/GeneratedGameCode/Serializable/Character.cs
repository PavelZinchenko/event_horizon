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
	public class CharacterSerializable : SerializableItem
	{
		public string Name;
		public string AvatarIcon;
		public int Faction;
		public int Inventory;
		public int Fleet;
		public int Relations;
		public bool IsUnique;
	}
}
