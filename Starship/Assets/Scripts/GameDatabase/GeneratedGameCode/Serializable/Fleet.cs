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
	public class FleetSerializable : SerializableItem
	{
		public FactionFilterSerializable Factions;
		public int LevelBonus;
		public bool NoRandomShips;
		public int CombatTimeLimit;
		public RewardCondition LootCondition;
		public RewardCondition ExpCondition;
		public int[] SpecificShips;
	}
}
