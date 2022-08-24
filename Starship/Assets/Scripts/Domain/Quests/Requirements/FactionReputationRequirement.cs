using Services.Localization;
using Session;
using UnityEngine.Assertions;

namespace Domain.Quests
{
    public class FactionReputationRequirement : IRequirements
    {
        public FactionReputationRequirement(int starId, int minValue, int maxValue, ISessionData session)
        {
            if (minValue > maxValue)
            {
                if (maxValue == 0) maxValue = int.MaxValue;
                else if (minValue == 0) minValue = int.MinValue;
            }

            Assert.IsTrue(minValue <= maxValue);

            _minValue = minValue;
            _maxValue = maxValue;
            _starId = starId;
            _session = session;
        }

        public bool IsMet
        {
            get
            {
                var starId = _starId > 0 ? _starId : _session.StarMap.PlayerPosition;
                var value = _session.Quests.GetFactionRelations(starId);
                return value >= _minValue && value <= _maxValue;
            }
        }

        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
#if UNITY_EDITOR
            return "FACTION RELATIONS " + _starId + " in [" + _minValue + "," + _maxValue + "]";
#else
            return string.Empty;
#endif
        }

        public int BeaconPosition { get { return -1; } }

        private readonly int _minValue;
        private readonly int _maxValue;
        private readonly int _starId;
        private readonly ISessionData _session;
    }
}
