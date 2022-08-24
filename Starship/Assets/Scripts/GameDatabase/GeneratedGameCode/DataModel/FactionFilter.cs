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
	public partial class RequiredFactions
	{
		partial void OnDataDeserialized(FactionFilterSerializable serializable, Database.Loader loader);

		public static RequiredFactions Create(FactionFilterSerializable serializable, Database.Loader loader)
		{
			return new RequiredFactions(serializable, loader);
		}

		private RequiredFactions(FactionFilterSerializable serializable, Database.Loader loader)
		{
			Type = serializable.Type;
			List = new ImmutableCollection<Faction>(serializable.List?.Select(item => loader.GetFaction(new ItemId<Faction>(item), true)));

			OnDataDeserialized(serializable, loader);
		}

		public FactionFilterType Type { get; private set; }
		public ImmutableCollection<Faction> List { get; private set; }

		public static RequiredFactions DefaultValue { get; private set; }
	}
}
