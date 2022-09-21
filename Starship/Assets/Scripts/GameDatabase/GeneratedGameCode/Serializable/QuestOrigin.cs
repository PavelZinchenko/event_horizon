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
	public struct QuestOriginSerializable
	{
		public QuestOriginType Type;
		public FactionFilterSerializable Factions;
		public int MinDistance;
		public int MaxDistance;
		public int MinRelations;
		public int MaxRelations;
	}
}
