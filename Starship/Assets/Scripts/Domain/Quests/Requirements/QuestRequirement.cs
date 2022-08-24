using System;
using GameDatabase.DataModel;
using Services.Localization;
using Session;

namespace Domain.Quests
{
    public class QuestRequirement : IRequirements
    {
        public enum RequiredStatus
        {
            Active,
            Completed,
        }

        public QuestRequirement(QuestModel quest, RequiredStatus status, ISessionData session)
        {
            _quest = quest;
            _status = status;
            _session = session;
        }

        public bool IsMet
        {
            get
            {
                switch (_status)
                {
                    case RequiredStatus.Active:
                        return _session.Quests.IsQuestActive(_quest.Id.Value);
                    case RequiredStatus.Completed:
                        return _session.Quests.HasBeenCompleted(_quest.Id.Value);
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
#if UNITY_EDITOR
            return "COMPLETE QUEST " + _quest.Id;
#else
            return string.Empty;
#endif
        }

        public int BeaconPosition { get { return -1; } }

        private readonly QuestModel _quest;
        private readonly RequiredStatus _status;
        private readonly ISessionData _session;
    }
}
