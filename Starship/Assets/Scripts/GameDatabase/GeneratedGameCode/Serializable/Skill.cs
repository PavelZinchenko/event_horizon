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
	public class SkillSerializable : SerializableItem
	{
		public string Name;
		public string Icon;
		public string Description;
		public float BaseRequirement;
		public float RequirementPerLevel;
		public float BasePrice;
		public float PricePerLevel;
		public int MaxLevel;
	}
}
