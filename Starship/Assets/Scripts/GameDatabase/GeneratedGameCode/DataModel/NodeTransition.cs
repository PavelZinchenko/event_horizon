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
	public partial class NodeTransition
	{
		partial void OnDataDeserialized(NodeTransitionSerializable serializable, Database.Loader loader);

		public static NodeTransition Create(NodeTransitionSerializable serializable, Database.Loader loader)
		{
			return new NodeTransition(serializable, loader);
		}

		private NodeTransition(NodeTransitionSerializable serializable, Database.Loader loader)
		{
			TargetNode = UnityEngine.Mathf.Clamp(serializable.TargetNode, 1, 2147483647);
			Requirement = Requirement.Create(serializable.Requirement, loader);
			Weight = UnityEngine.Mathf.Clamp(serializable.Weight, 0f, 1000f);

			OnDataDeserialized(serializable, loader);
		}

		public int TargetNode { get; private set; }
		public Requirement Requirement { get; private set; }
		public float Weight { get; private set; }

		public static NodeTransition DefaultValue { get; private set; }
	}
}
