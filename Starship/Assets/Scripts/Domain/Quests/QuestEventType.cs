using Combat.Domain;
using Utils;

namespace Domain.Quests
{
    public enum QuestEventType
    {
        ArrivedAtStarSystem,
        NewStarSystemExplored,
        ActionButtonPressed,
        BeaconActivated,
        LocalEncounter,
        CombatCompleted,
        OccupantsDefeated,
        FactionMissionAccepted,
        Timer,
    }

    public class QuestEventSignal : SmartWeakSignal<IQuestEventData>
    {
        public class Trigger : TriggerBase { }
    }

    public interface IQuestEventData
    {
        QuestEventType Type { get; }
    }

    public class SimpleEventData : IQuestEventData
    {
        public SimpleEventData(QuestEventType type)
        {
            _type = type;
        }

        public QuestEventType Type { get { return _type; } }

        private readonly QuestEventType _type;
    }

    public class StarEventData : IQuestEventData
    {
        public StarEventData(QuestEventType type, int starId)
        {
            _type = type;
            _starId = starId;
        }

        public int StarId { get { return _starId; } }
        public QuestEventType Type { get { return _type; } }

        private readonly QuestEventType _type;
        private readonly int _starId;
    }

    public class ButtonPressedEventData : IQuestEventData
    {
        public ButtonPressedEventData(INode context, int id)
        {
            Context = context;
            Id = id;
        }

        public QuestEventType Type { get { return QuestEventType.ActionButtonPressed; } }

        public readonly int Id;
        public readonly INode Context;
    }

    public class BeaconEventData : IQuestEventData
    {
        public BeaconEventData(int starId, int seed)
        {
            StarId = starId;
            Seed = seed;
        }

        public QuestEventType Type { get { return QuestEventType.BeaconActivated; } }

        public readonly int StarId;
        public readonly int Seed;
    }

    public class LocalEncounterEventData : IQuestEventData
    {
        public LocalEncounterEventData(int starId, int seed)
        {
            StarId = starId;
            Seed = seed;
        }

        public QuestEventType Type { get { return QuestEventType.LocalEncounter; } }

        public readonly int StarId;
        public readonly int Seed;
    }

    public class CombatEventData : IQuestEventData
    {
        public CombatEventData(ICombatModel combatModel)
        {
            CombatModel = combatModel;
        }

        public QuestEventType Type { get { return QuestEventType.CombatCompleted; } }
        public readonly ICombatModel CombatModel;
    }

    public static class QuestEventFactory
    {
        public static IQuestEventData CreateBeaconEventContext(int starId, int seed)
        {
            return new BeaconEventData(starId, seed);
        }

        public static IQuestEventData CreateContext(QuestEventType type)
        {
            return new SimpleEventData(type);
        }
    }
}
