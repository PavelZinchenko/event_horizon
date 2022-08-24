using System.Collections.Generic;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public interface INode
    {
        int Id { get; }
        NodeType Type { get; }

        string GetRequirementsText(ILocalization localization);
        bool TryGetBeacons(ICollection<int> beacons);

        void Initialize();

        bool TryProceed(out INode target);
        bool TryProcessEvent(IQuestEventData data, out INode target);

        bool ActionRequired { get; }
        bool TryInvokeAction(IQuestActionProcessor processor);
    }
}
