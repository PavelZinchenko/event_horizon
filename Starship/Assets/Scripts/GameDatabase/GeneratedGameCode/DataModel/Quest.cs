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
	public partial class QuestModel
	{
		partial void OnDataDeserialized(QuestSerializable serializable, Database.Loader loader);

		public static QuestModel Create(QuestSerializable serializable, Database.Loader loader)
		{
			return new QuestModel(serializable, loader);
		}

		private QuestModel(QuestSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<QuestModel>(serializable.Id);
			loader.AddQuest(serializable.Id, this);

			Name = serializable.Name;
			QuestType = serializable.QuestType;
			StartCondition = serializable.StartCondition;
			Weight = UnityEngine.Mathf.Clamp(serializable.Weight, 0f, 1000f);
			Origin = QuestOrigin.Create(serializable.Origin, loader);
			Requirement = Requirement.Create(serializable.Requirement, loader);
			Level = UnityEngine.Mathf.Clamp(serializable.Level, 0, 1000);
			Nodes = new ImmutableCollection<Node>(serializable.Nodes?.Select(item => Node.Create(item, loader)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<QuestModel> Id;

		public string Name { get; private set; }
		public QuestType QuestType { get; private set; }
		public StartCondition StartCondition { get; private set; }
		public float Weight { get; private set; }
		public QuestOrigin Origin { get; private set; }
		public Requirement Requirement { get; private set; }
		public int Level { get; private set; }
		public ImmutableCollection<Node> Nodes { get; private set; }

		public static QuestModel DefaultValue { get; private set; }
	}
}
