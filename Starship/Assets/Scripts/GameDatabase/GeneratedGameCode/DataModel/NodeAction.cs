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
	public partial class NodeAction
	{
		partial void OnDataDeserialized(NodeActionSerializable serializable, Database.Loader loader);

		public static NodeAction Create(NodeActionSerializable serializable, Database.Loader loader)
		{
			return new NodeAction(serializable, loader);
		}

		private NodeAction(NodeActionSerializable serializable, Database.Loader loader)
		{
			TargetNode = UnityEngine.Mathf.Clamp(serializable.TargetNode, 1, 1000);
			Requirement = Requirement.Create(serializable.Requirement, loader);
			ButtonText = serializable.ButtonText;

			OnDataDeserialized(serializable, loader);
		}

		public int TargetNode { get; private set; }
		public Requirement Requirement { get; private set; }
		public string ButtonText { get; private set; }

		public static NodeAction DefaultValue { get; private set; }
	}
}
