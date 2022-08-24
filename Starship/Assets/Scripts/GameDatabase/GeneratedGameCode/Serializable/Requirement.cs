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
	public struct RequirementSerializable
	{
		public RequirementType Type;
		public int ItemId;
		public int MinValue;
		public int MaxValue;
		public int Character;
		public int Faction;
		public LootContentSerializable Loot;
		public RequirementSerializable[] Requirements;
	}
}
