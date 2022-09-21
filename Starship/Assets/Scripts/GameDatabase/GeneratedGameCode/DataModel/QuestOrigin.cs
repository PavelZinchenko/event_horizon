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
	public partial class QuestOrigin
	{
		partial void OnDataDeserialized(QuestOriginSerializable serializable, Database.Loader loader);

		public static QuestOrigin Create(QuestOriginSerializable serializable, Database.Loader loader)
		{
			return new QuestOrigin(serializable, loader);
		}

		private QuestOrigin(QuestOriginSerializable serializable, Database.Loader loader)
		{
			Type = serializable.Type;
			Factions = RequiredFactions.Create(serializable.Factions, loader);
			MinDistance = UnityEngine.Mathf.Clamp(serializable.MinDistance, 0, 9999);
			MaxDistance = UnityEngine.Mathf.Clamp(serializable.MaxDistance, 0, 9999);
			MinRelations = UnityEngine.Mathf.Clamp(serializable.MinRelations, -2147483648, 2147483647);
			MaxRelations = UnityEngine.Mathf.Clamp(serializable.MaxRelations, -2147483648, 2147483647);

			OnDataDeserialized(serializable, loader);
		}

		public QuestOriginType Type { get; private set; }
		public RequiredFactions Factions { get; private set; }
		public int MinDistance { get; private set; }
		public int MaxDistance { get; private set; }
		public int MinRelations { get; private set; }
		public int MaxRelations { get; private set; }

		public static QuestOrigin DefaultValue { get; private set; }
	}
}
