using GameDatabase.DataModel;
using Services.Localization;
using Session;

namespace Domain.Quests
{
    public class ArtifactRequirement : IRequirements
    {
        public ArtifactRequirement(QuestItem questItem, int amount, ISessionData session)
        {
            _questItem = questItem;
            _amount = amount;
            _session = session;
        }

        public bool IsMet { get { return _session.Resources.Resources.GetQuantity(_questItem.Id.Value) >= _amount; } }
        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
            return _amount > 1
                ? _amount + "x " + localization.GetString(_questItem.Name)
                : localization.GetString(_questItem.Name);
        }

        public int BeaconPosition { get { return -1; } }

        private readonly int _amount;
        private readonly QuestItem _questItem;
        private readonly ISessionData _session;
    }
}
