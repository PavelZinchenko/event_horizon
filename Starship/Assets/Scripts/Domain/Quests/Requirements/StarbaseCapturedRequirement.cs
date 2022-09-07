using GameServices.Player;
using Services.Localization;

namespace Domain.Quests
{
    public class StarbaseCapturedRequirement : IRequirements
    {
        public StarbaseCapturedRequirement (  MotherShip motherShip )
        {
            _motherShip = motherShip;
        }

        public bool IsMet
        {
            get
            {
                return _motherShip.CurrentStar.Region.IsCaptured && _motherShip.CurrentStar.Region.Faction.Id.Value != 0;
            }
        }

        public int BeaconPosition => -1;

        public bool CanStart ( int starId, int seed ) { return IsMet; }

        public string GetDescription ( ILocalization localization )
        {
            return "$StarbaseMustBeCaptured";
        }

        private readonly MotherShip _motherShip;
    }
}

