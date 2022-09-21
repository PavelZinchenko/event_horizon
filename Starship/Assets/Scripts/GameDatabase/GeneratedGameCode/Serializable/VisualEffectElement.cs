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
	public struct VisualEffectElementSerializable
	{
		public VisualEffectType Type;
		public string Image;
		public ColorMode ColorMode;
		public string Color;
		public float Size;
		public float StartTime;
		public float Lifetime;
	}
}
