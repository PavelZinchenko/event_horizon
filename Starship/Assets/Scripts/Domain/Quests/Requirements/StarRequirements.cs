using GameServices.Player;
using Services.Localization;

namespace Domain.Quests
{
    public class StarRequirements : IRequirements
    {
        public StarRequirements(int starId, MotherShip motherShip)
        {
            _motherShip = motherShip;
            _starId = starId;
        }

        public bool IsMet
        {
            get
            {
                return _motherShip.Position == _starId && !_motherShip.CurrentStar.Occupant.IsAggressive;
            }
        }

        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Condition_GoToStar", Model.Generators.NameGenerator.GetStarName(_starId));
        }

        public int BeaconPosition { get { return _starId; } }

        private readonly int _starId;
        private readonly MotherShip _motherShip;
    }
}
