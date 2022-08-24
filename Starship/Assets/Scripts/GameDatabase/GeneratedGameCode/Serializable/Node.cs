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
	public struct NodeSerializable
	{
		public int Id;
		public NodeType Type;
		public RequiredViewMode RequiredView;
		public string Message;
		public int DefaultTransition;
		public int FailureTransition;
		public int Enemy;
		public int Loot;
		public int Quest;
		public int Character;
		public int Faction;
		public int Value;
		public NodeActionSerializable[] Actions;
		public NodeTransitionSerializable[] Transitions;
	}
}
