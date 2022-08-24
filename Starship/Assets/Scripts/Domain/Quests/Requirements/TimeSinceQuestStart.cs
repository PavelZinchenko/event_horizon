using System;
using Services.InternetTime;
using Services.Localization;
using Session;

namespace Domain.Quests
{
    public class TimeSinceQuestStart : IRequirements
    {
        public TimeSinceQuestStart(int questId, int starId, ISessionData session, long totalTicks, GameTime gameTime)
        {
            _questId = questId;
            _starId = starId;
            _totalTicks = totalTicks;
            _session = session;
            _gameTime = gameTime;
        }

        public bool IsMet => ElapsedTime >= _totalTicks;

        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
            var seconds = (_totalTicks - ElapsedTime) / TimeSpan.TicksPerSecond;
            return TimeSinceLastCompletion.TimeToString(localization, seconds);
        }

        public int BeaconPosition => -1;

        private long ElapsedTime => _gameTime.TotalPlayTime - _session.Quests.QuestStartTime(_questId, _starId);

        private readonly long _totalTicks;
        private readonly int _questId;
        private readonly int _starId;
        private readonly GameTime _gameTime;
        private readonly ISessionData _session;
    }
}