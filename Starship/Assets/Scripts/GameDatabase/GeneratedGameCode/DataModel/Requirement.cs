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
	public abstract partial class Requirement
	{
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

		public static Requirement Create(RequirementSerializable serializable, Database.Loader loader)
		{
			switch (serializable.Type)
		    {
				case RequirementType.Empty:
					return new Requirement_Empty(serializable, loader);
				case RequirementType.Any:
					return new Requirement_Any(serializable, loader);
				case RequirementType.All:
					return new Requirement_All(serializable, loader);
				case RequirementType.None:
					return new Requirement_None(serializable, loader);
				case RequirementType.PlayerPosition:
					return new Requirement_PlayerPosition(serializable, loader);
				case RequirementType.RandomStarSystem:
					return new Requirement_RandomStarSystem(serializable, loader);
				case RequirementType.AggressiveOccupants:
					return new Requirement_AggressiveOccupants(serializable, loader);
				case RequirementType.QuestCompleted:
					return new Requirement_QuestCompleted(serializable, loader);
				case RequirementType.QuestActive:
					return new Requirement_QuestActive(serializable, loader);
				case RequirementType.CharacterRelations:
					return new Requirement_CharacterRelations(serializable, loader);
				case RequirementType.FactionRelations:
					return new Requirement_FactionRelations(serializable, loader);
				case RequirementType.StarbaseCaptured:
					return new Requirement_StarbaseCaptured(serializable, loader);
				case RequirementType.Faction:
					return new Requirement_Faction(serializable, loader);
				case RequirementType.HaveQuestItem:
					return new Requirement_HaveQuestItem(serializable, loader);
				case RequirementType.HaveItem:
					return new Requirement_HaveItem(serializable, loader);
				case RequirementType.HaveItemById:
					return new Requirement_HaveItemById(serializable, loader);
				case RequirementType.ComeToOrigin:
					return new Requirement_ComeToOrigin(serializable, loader);
				case RequirementType.TimeSinceQuestStart:
					return new Requirement_TimeSinceQuestStart(serializable, loader);
				case RequirementType.TimeSinceLastCompletion:
					return new Requirement_TimeSinceLastCompletion(serializable, loader);
				default:
                    throw new DatabaseException("Requirement: Invalid content type - " + serializable.Type);
			}
		}

		public abstract T Create<T>(IRequirementFactory<T> factory);

		protected Requirement(RequirementSerializable serializable, Database.Loader loader)
		{
			Type = serializable.Type;

			OnDataDeserialized(serializable, loader);
		}

		public RequirementType Type { get; private set; }

		public static Requirement DefaultValue { get; private set; }
	}

	public interface IRequirementFactory<T>
    {
	    T Create(Requirement_Empty content);
	    T Create(Requirement_Any content);
	    T Create(Requirement_All content);
	    T Create(Requirement_None content);
	    T Create(Requirement_PlayerPosition content);
	    T Create(Requirement_RandomStarSystem content);
	    T Create(Requirement_AggressiveOccupants content);
	    T Create(Requirement_QuestCompleted content);
	    T Create(Requirement_QuestActive content);
	    T Create(Requirement_CharacterRelations content);
	    T Create(Requirement_FactionRelations content);
	    T Create(Requirement_StarbaseCaptured content);
	    T Create(Requirement_Faction content);
	    T Create(Requirement_HaveQuestItem content);
	    T Create(Requirement_HaveItem content);
	    T Create(Requirement_HaveItemById content);
	    T Create(Requirement_ComeToOrigin content);
	    T Create(Requirement_TimeSinceQuestStart content);
	    T Create(Requirement_TimeSinceLastCompletion content);
    }

    public partial class Requirement_Empty : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_Empty(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Requirement_Any : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_Any(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Requirements = new ImmutableCollection<Requirement>(serializable.Requirements?.Select(item => Requirement.Create(item, loader)));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public ImmutableCollection<Requirement> Requirements { get; private set; }
    }
    public partial class Requirement_All : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_All(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Requirements = new ImmutableCollection<Requirement>(serializable.Requirements?.Select(item => Requirement.Create(item, loader)));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public ImmutableCollection<Requirement> Requirements { get; private set; }
    }
    public partial class Requirement_None : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_None(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Requirements = new ImmutableCollection<Requirement>(serializable.Requirements?.Select(item => Requirement.Create(item, loader)));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public ImmutableCollection<Requirement> Requirements { get; private set; }
    }
    public partial class Requirement_PlayerPosition : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_PlayerPosition(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			MinValue = UnityEngine.Mathf.Clamp(serializable.MinValue, 0, 10000);
			MaxValue = UnityEngine.Mathf.Clamp(serializable.MaxValue, 0, 10000);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int MinValue { get; private set; }
		public int MaxValue { get; private set; }
    }
    public partial class Requirement_RandomStarSystem : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_RandomStarSystem(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			MinValue = UnityEngine.Mathf.Clamp(serializable.MinValue, 0, 10000);
			MaxValue = UnityEngine.Mathf.Clamp(serializable.MaxValue, 0, 10000);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int MinValue { get; private set; }
		public int MaxValue { get; private set; }
    }
    public partial class Requirement_AggressiveOccupants : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_AggressiveOccupants(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Requirement_QuestCompleted : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_QuestCompleted(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Quest = loader.GetQuest(new ItemId<QuestModel>(serializable.ItemId));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public QuestModel Quest { get; private set; }
    }
    public partial class Requirement_QuestActive : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_QuestActive(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Quest = loader.GetQuest(new ItemId<QuestModel>(serializable.ItemId));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public QuestModel Quest { get; private set; }
    }
    public partial class Requirement_CharacterRelations : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_CharacterRelations(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			MinValue = UnityEngine.Mathf.Clamp(serializable.MinValue, -100, 100);
			MaxValue = UnityEngine.Mathf.Clamp(serializable.MaxValue, -100, 100);
			Character = loader.GetCharacter(new ItemId<Character>(serializable.Character));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int MinValue { get; private set; }
		public int MaxValue { get; private set; }
		public Character Character { get; private set; }
    }
    public partial class Requirement_FactionRelations : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_FactionRelations(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			MinValue = UnityEngine.Mathf.Clamp(serializable.MinValue, -100, 100);
			MaxValue = UnityEngine.Mathf.Clamp(serializable.MaxValue, -100, 100);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int MinValue { get; private set; }
		public int MaxValue { get; private set; }
    }
    public partial class Requirement_StarbaseCaptured : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_StarbaseCaptured(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Requirement_Faction : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_Faction(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public Faction Faction { get; private set; }
    }
    public partial class Requirement_HaveQuestItem : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_HaveQuestItem(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			QuestItem = loader.GetQuestItem(new ItemId<QuestItem>(serializable.ItemId));
			Amount = UnityEngine.Mathf.Clamp(serializable.MinValue, 1, 1000000);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public QuestItem QuestItem { get; private set; }
		public int Amount { get; private set; }
    }
    public partial class Requirement_HaveItem : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_HaveItem(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Loot = LootContent.Create(serializable.Loot, loader);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public LootContent Loot { get; private set; }
    }
    public partial class Requirement_HaveItemById : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_HaveItemById(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Loot = loader.GetLoot(new ItemId<LootModel>(serializable.ItemId));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public LootModel Loot { get; private set; }
    }
    public partial class Requirement_ComeToOrigin : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_ComeToOrigin(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class Requirement_TimeSinceQuestStart : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_TimeSinceQuestStart(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Minutes = UnityEngine.Mathf.Clamp(serializable.MinValue, 0, 999999);
			Hours = UnityEngine.Mathf.Clamp(serializable.MaxValue, 0, 999999);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Minutes { get; private set; }
		public int Hours { get; private set; }
    }
    public partial class Requirement_TimeSinceLastCompletion : Requirement
    {
		partial void OnDataDeserialized(RequirementSerializable serializable, Database.Loader loader);

  		public Requirement_TimeSinceLastCompletion(RequirementSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Minutes = UnityEngine.Mathf.Clamp(serializable.MinValue, 0, 999999);
			Hours = UnityEngine.Mathf.Clamp(serializable.MaxValue, 0, 999999);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IRequirementFactory<T> factory)
        {
            return factory.Create(this);
        }

		public int Minutes { get; private set; }
		public int Hours { get; private set; }
    }

}

