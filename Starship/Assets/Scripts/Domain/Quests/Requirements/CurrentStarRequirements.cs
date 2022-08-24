using GameServices.Player;
using Services.Localization;

namespace Domain.Quests
{
    public class CurrentStarRequirements : IRequirements
    {
        public CurrentStarRequirements(int minDistance, int maxDistance, MotherShip motherShip)
        {
            _motherShip = motherShip;

            _minDistance = minDistance;
            _maxDistance = maxDistance;
        }

        public bool IsMet
        {
            get
            {
                var star = _motherShip.CurrentStar;
                var level = star.Level;
                if (level < _minDistance || level > _maxDistance) return false;
                return !star.Occupant.IsAggressive;
            }
        }

        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
#if UNITY_EDITOR
            return "POSITION: " + _minDistance + " - " + _maxDistance;
#else
            return string.Empty;
#endif
        }

        public int BeaconPosition { get { return -1; } }

        private readonly int _minDistance;
        private readonly int _maxDistance;
        private readonly MotherShip _motherShip;
    }
}
