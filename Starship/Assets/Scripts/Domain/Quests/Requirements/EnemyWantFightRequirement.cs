using GameServices.Player;
using Services.Localization;

namespace Domain.Quests
{
    public class EnemiesWantFightRequirements : IRequirements
    {
        public EnemiesWantFightRequirements(MotherShip motherShip)
        {
            _motherShip = motherShip;
        }

        public bool IsMet { get { return _motherShip.CurrentStar.Occupant.CanBeAggressive; } }
        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization) { return string.Empty; }
        public int BeaconPosition { get { return -1; } }

        private readonly MotherShip _motherShip;
    }
}
