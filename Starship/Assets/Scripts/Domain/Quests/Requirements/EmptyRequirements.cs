using Services.Localization;

namespace Domain.Quests
{
    public class EmptyRequirements : IRequirements
    {
        public bool IsMet { get { return true; } }
        public bool CanStart(int starId, int seed) { return true; }
        public string GetDescription(ILocalization localization) { return string.Empty; }
        public int BeaconPosition { get { return -1; } }

        public static readonly EmptyRequirements Instance = new EmptyRequirements();
    }
}
