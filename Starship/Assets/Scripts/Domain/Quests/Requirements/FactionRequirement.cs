using GameDatabase.DataModel;
using GameServices.Player;
using Services.Localization;

namespace Domain.Quests
{
    public class FactionRequirements : IRequirements
    {
        public FactionRequirements(Faction faction, MotherShip motherShip)
        {
            _faction = faction;
            _motherShip = motherShip;
        }

        public bool IsMet { get { return _motherShip.CurrentStar.Region.Faction == _faction; } }

        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
#if UNITY_EDITOR
            return "FACTION: " + _faction;
#else
            return string.Empty;
#endif
        }

        public int BeaconPosition { get { return -1; } }

        private readonly Faction _faction;
        private readonly MotherShip _motherShip;
    }
}
