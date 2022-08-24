//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System.Linq;
using GameDatabase.Enums;
using GameDatabase.Serializable;
using GameDatabase.Model;

namespace GameDatabase.DataModel
{
	public partial class ComponentRestrictions
	{
		partial void OnDataDeserialized(ComponentRestrictionsSerializable serializable, Database.Loader loader);

		public static ComponentRestrictions Create(ComponentRestrictionsSerializable serializable, Database.Loader loader)
		{
			return new ComponentRestrictions(serializable, loader);
		}

		private ComponentRestrictions(ComponentRestrictionsSerializable serializable, Database.Loader loader)
		{
			ShipSizes = new ImmutableSet<SizeClass>(serializable.ShipSizes);
			NotForOrganicShips = serializable.NotForOrganicShips;
			NotForMechanicShips = serializable.NotForMechanicShips;
			UniqueComponentTag = serializable.UniqueComponentTag;

			OnDataDeserialized(serializable, loader);
		}

		public ImmutableSet<SizeClass> ShipSizes { get; private set; }
		public bool NotForOrganicShips { get; private set; }
		public bool NotForMechanicShips { get; private set; }
		public string UniqueComponentTag { get; private set; }

		public static ComponentRestrictions DefaultValue { get; private set; }
	}
}
