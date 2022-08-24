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
	public class BulletPrefabSerializable : SerializableItem
	{
		public BulletShape Shape;
		public string Image;
		public float Size;
		public float Margins;
		public string MainColor;
		public ColorMode MainColorMode;
		public string SecondColor;
		public ColorMode SecondColorMode;
	}
}
