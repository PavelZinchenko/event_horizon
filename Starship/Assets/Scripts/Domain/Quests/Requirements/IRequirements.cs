using Services.Localization;

namespace Domain.Quests
{
    public interface INodeRequirements
    {
        bool IsMet { get; }
        string GetDescription(ILocalization localization);
        int BeaconPosition { get; }
    }

    public interface IQuestRequirements
    {
        bool CanStart(int starId, int seed);
    }

    public interface IRequirements : INodeRequirements, IQuestRequirements
    {
    }
}
