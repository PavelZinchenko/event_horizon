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
	public abstract partial class Node
	{
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

		public static Node Create(NodeSerializable serializable, Database.Loader loader)
		{
			switch (serializable.Type)
		    {
				case NodeType.Undefined:
					return new Node_Undefined(serializable, loader);
				case NodeType.ComingSoon:
					return new Node_ComingSoon(serializable, loader);
				case NodeType.ShowDialog:
					return new Node_ShowDialog(serializable, loader);
				case NodeType.OpenShipyard:
					return new Node_OpenShipyard(serializable, loader);
				case NodeType.Switch:
					return new Node_Switch(serializable, loader);
				case NodeType.Random:
					return new Node_Random(serializable, loader);
				case NodeType.Condition:
					return new Node_Condition(serializable, loader);
				case NodeType.AttackFleet:
					return new Node_AttackFleet(serializable, loader);
				case NodeType.AttackOccupants:
					return new Node_AttackOccupants(serializable, loader);
				case NodeType.DestroyOccupants:
					return new Node_DestroyOccupants(serializable, loader);
				case NodeType.SuppressOccupants:
					return new Node_SuppressOccupants(serializable, loader);
				case NodeType.Retreat:
					return new Node_Retreat(serializable, loader);
				case NodeType.ReceiveItem:
					return new Node_ReceiveItem(serializable, loader);
				case NodeType.RemoveItem:
					return new Node_RemoveItem(serializable, loader);
				case NodeType.Trade:
					return new Node_Trade(serializable, loader);
				case NodeType.CompleteQuest:
					return new Node_CompleteQuest(serializable, loader);
				case NodeType.FailQuest:
					return new Node_FailQuest(serializable, loader);
				case NodeType.CancelQuest:
					return new Node_CancelQuest(serializable, loader);
				case NodeType.StartQuest:
					return new Node_StartQuest(serializable, loader);
				case NodeType.SetCharacterRelations:
					return new Node_SetCharacterRelations(serializable, loader);
				case NodeType.SetFactionRelations:
					return new Node_SetFactionRelations(serializable, loader);
				case NodeType.ChangeCharacterRelations:
					return new Node_ChangeCharacterRelations(serializable, loader);
				case NodeType.ChangeFactionRelations:
					return new Node_ChangeFactionRelations(serializable, loader);
				case NodeType.CaptureStarBase:
					return new Node_CaptureStarBase(serializable, loader);
				case NodeType.LiberateStarBase:
					return new Node_LiberateStarBase(serializable, loader);
				case NodeType.ChangeFaction:
					return new Node_ChangeFaction(serializable, loader);
				default:
                    throw new DatabaseException("Node: Invalid content type - " + serializable.Type);
			}
		}

		public abstract T Create<T>(INodeFactory<T> factory);

		protected Node(NodeSerializable serializable, Database.Loader loader)
		{
			Id = UnityEngine.Mathf.Clamp(serializable.Id, 1, 2147483647);
			Type = serializable.Type;

			OnDataDeserialized(serializable, loader);
		}

		public int Id { get; private set; }
		public NodeType Type { get; private set; }

		public static Node DefaultValue { get; private set; }
	}

	public interface INodeFactory<T>
    {
	    T Create(Node_Undefined content);
	    T Create(Node_ComingSoon content);
	    T Create(Node_ShowDialog content);
	    T Create(Node_OpenShipyard content);
	    T Create(Node_Switch content);
	    T Create(Node_Random content);
	    T Create(Node_Condition content);
	    T Create(Node_AttackFleet content);
	    T Create(Node_AttackOccupants content);
	    T Create(Node_DestroyOccupants content);
	    T Create(Node_SuppressOccupants content);
	    T Create(Node_Retreat content);
	    T Create(Node_ReceiveItem content);
	    T Create(Node_RemoveItem content);
	    T Create(Node_Trade content);
	    T Create(Node_CompleteQuest content);
	    T Create(Node_FailQuest content);
	    T Create(Node_CancelQuest content);
	    T Create(Node_StartQuest content);
	    T Create(Node_SetCharacterRelations content);
	    T Create(Node_SetFactionRelations content);
	    T Create(Node_ChangeCharacterRelations content);
	    T Create(Node_ChangeFactionRelations content);
	    T Create(Node_CaptureStarBase content);
	    T Create(Node_LiberateStarBase content);
	    T Create(Node_ChangeFaction content);
    }

    public partial class Node_Undefined : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_Undefined(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Node_ComingSoon : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_ComingSoon(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Node_ShowDialog : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_ShowDialog(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			RequiredView = serializable.RequiredView;
			Message = serializable.Message;
			Enemy = loader.GetFleet(new ItemId<Fleet>(serializable.Enemy));
			Loot = loader.GetLoot(new ItemId<LootModel>(serializable.Loot));
			Character = loader.GetCharacter(new ItemId<Character>(serializable.Character));
			Actions = new ImmutableCollection<NodeAction>(serializable.Actions?.Select(item => NodeAction.Create(item, loader)));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public RequiredViewMode RequiredView { get; private set; }
		public string Message { get; private set; }
		public Fleet Enemy { get; private set; }
		public LootModel Loot { get; private set; }
		public Character Character { get; private set; }
		public ImmutableCollection<NodeAction> Actions { get; private set; }
    }
    public partial class Node_OpenShipyard : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_OpenShipyard(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));
			Level = UnityEngine.Mathf.Clamp(serializable.Value, 0, 10000);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public Faction Faction { get; private set; }
		public int Level { get; private set; }
    }
    public partial class Node_Switch : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_Switch(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Message = serializable.Message;
			DefaultTransition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Transitions = new ImmutableCollection<NodeTransition>(serializable.Transitions?.Select(item => NodeTransition.Create(item, loader)));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public string Message { get; private set; }
		public int DefaultTransition { get; private set; }
		public ImmutableCollection<NodeTransition> Transitions { get; private set; }
    }
    public partial class Node_Random : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_Random(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Message = serializable.Message;
			DefaultTransition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Transitions = new ImmutableCollection<NodeTransition>(serializable.Transitions?.Select(item => NodeTransition.Create(item, loader)));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public string Message { get; private set; }
		public int DefaultTransition { get; private set; }
		public ImmutableCollection<NodeTransition> Transitions { get; private set; }
    }
    public partial class Node_Condition : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_Condition(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Message = serializable.Message;
			Transitions = new ImmutableCollection<NodeTransition>(serializable.Transitions?.Select(item => NodeTransition.Create(item, loader)));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public string Message { get; private set; }
		public ImmutableCollection<NodeTransition> Transitions { get; private set; }
    }
    public partial class Node_AttackFleet : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_AttackFleet(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			VictoryTransition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			FailureTransition = UnityEngine.Mathf.Clamp(serializable.FailureTransition, 1, 2147483647);
			Enemy = loader.GetFleet(new ItemId<Fleet>(serializable.Enemy));
			Loot = loader.GetLoot(new ItemId<LootModel>(serializable.Loot));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int VictoryTransition { get; private set; }
		public int FailureTransition { get; private set; }
		public Fleet Enemy { get; private set; }
		public LootModel Loot { get; private set; }
    }
    public partial class Node_AttackOccupants : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_AttackOccupants(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			VictoryTransition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			FailureTransition = UnityEngine.Mathf.Clamp(serializable.FailureTransition, 1, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int VictoryTransition { get; private set; }
		public int FailureTransition { get; private set; }
    }
    public partial class Node_DestroyOccupants : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_DestroyOccupants(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
    }
    public partial class Node_SuppressOccupants : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_SuppressOccupants(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
    }
    public partial class Node_Retreat : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_Retreat(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
    }
    public partial class Node_ReceiveItem : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_ReceiveItem(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Loot = loader.GetLoot(new ItemId<LootModel>(serializable.Loot));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public LootModel Loot { get; private set; }
    }
    public partial class Node_RemoveItem : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_RemoveItem(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Loot = loader.GetLoot(new ItemId<LootModel>(serializable.Loot));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public LootModel Loot { get; private set; }
    }
    public partial class Node_Trade : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_Trade(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Loot = loader.GetLoot(new ItemId<LootModel>(serializable.Loot));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public LootModel Loot { get; private set; }
    }
    public partial class Node_CompleteQuest : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_CompleteQuest(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Node_FailQuest : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_FailQuest(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Node_CancelQuest : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_CancelQuest(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Node_StartQuest : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_StartQuest(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Quest = loader.GetQuest(new ItemId<QuestModel>(serializable.Quest));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public QuestModel Quest { get; private set; }
    }
    public partial class Node_SetCharacterRelations : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_SetCharacterRelations(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Character = loader.GetCharacter(new ItemId<Character>(serializable.Character));
			Value = UnityEngine.Mathf.Clamp(serializable.Value, -2147483648, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public Character Character { get; private set; }
		public int Value { get; private set; }
    }
    public partial class Node_SetFactionRelations : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_SetFactionRelations(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Value = UnityEngine.Mathf.Clamp(serializable.Value, -2147483648, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public int Value { get; private set; }
    }
    public partial class Node_ChangeCharacterRelations : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_ChangeCharacterRelations(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Character = loader.GetCharacter(new ItemId<Character>(serializable.Character));
			Value = UnityEngine.Mathf.Clamp(serializable.Value, -2147483648, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public Character Character { get; private set; }
		public int Value { get; private set; }
    }
    public partial class Node_ChangeFactionRelations : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_ChangeFactionRelations(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Value = UnityEngine.Mathf.Clamp(serializable.Value, -2147483648, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public int Value { get; private set; }
    }
    public partial class Node_CaptureStarBase : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_CaptureStarBase(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
    }
    public partial class Node_LiberateStarBase : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_LiberateStarBase(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
    }
    public partial class Node_ChangeFaction : Node
    {
		partial void OnDataDeserialized(NodeSerializable serializable, Database.Loader loader);

  		public Node_ChangeFaction(NodeSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Transition = UnityEngine.Mathf.Clamp(serializable.DefaultTransition, 1, 2147483647);
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(INodeFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Transition { get; private set; }
		public Faction Faction { get; private set; }
    }

}

