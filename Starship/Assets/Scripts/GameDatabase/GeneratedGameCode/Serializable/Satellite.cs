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
	public class SatelliteSerializable : SerializableItem
	{
		public string Name;
		public string ModelImage;
		public float ModelScale;
		public SizeClass SizeClass;
		public string Layout;
		public BarrelSerializable[] Barrels;
	}
}
