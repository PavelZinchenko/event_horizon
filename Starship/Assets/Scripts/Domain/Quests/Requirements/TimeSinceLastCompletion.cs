using System;
using System.Text;
using Model.Regulations;
using Services.InternetTime;
using Services.Localization;
using Session;

namespace Domain.Quests
{
    public class TimeSinceLastCompletion : IRequirements
    {
        public TimeSinceLastCompletion(int questId, ISessionData session, long totalTicks, GameTime gameTime)
        {
            _questId = questId;
            _totalTicks = totalTicks;
            _session = session;
            _gameTime = gameTime;
        }

        public bool IsMet => ElapsedTime >= _totalTicks;

        public bool CanStart(int starId, int seed) { return IsMet; }

        public string GetDescription(ILocalization localization)
        {
            var seconds = (_totalTicks - ElapsedTime) / TimeSpan.TicksPerSecond;
            return TimeToString(localization, seconds);
        }

        public int BeaconPosition => -1;

        public static string TimeToString(ILocalization localization, long seconds)
        {
            if (seconds <= 0) return string.Empty;

            var text = localization.GetString("$TimeLeft");
            if (seconds > 24 * 60 * 60)
                text += localization.GetString("$TimeDays", seconds / 86400);
            else if (seconds > 60 * 60)
                text += localization.GetString("$TimeHours", seconds / 3600);
            else if (seconds > 60)
                text += localization.GetString("$TimeMinutes", seconds / 60);
            else
                text += localization.GetString("$TimeSeconds", seconds);

            return text;
        }

        private long ElapsedTime => _gameTime.TotalPlayTime - _session.Quests.LastCompletionTime(_questId);

        private readonly long _totalTicks;
        private readonly int _questId;
        private readonly GameTime _gameTime;
        private readonly ISessionData _session;
    }
}