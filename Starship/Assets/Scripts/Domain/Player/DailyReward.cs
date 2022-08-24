using System;
using System.Collections.Generic;
using Economy.Products;
using GameModel;
using GameServices;
using GameServices.Economy;
using GameServices.Settings;
using Services.InternetTime;
using Session;
using UnityEngine;
using Utils;

namespace Domain.Player
{
    public class DailyReward : GameServiceBase
    {
        public DailyReward(
            ISessionData session, 
            GameSettings gameSettings, 
            InternetTimeService internetTime, 
            ServerTimeReceivedSignal timeReceivedSignal,
            DailyRewardAwailableSignal.Trigger rewardAvailableTrigger,
            SessionDataLoadedSignal sessionDataLoadedSignal,
            SessionCreatedSignal sessionCreatedSignal,
            LootGenerator lootGenerator)
            : base(sessionDataLoadedSignal, sessionCreatedSignal)
        {
            _session = session;
            _lootGenerator = lootGenerator;
            _gameSettings = gameSettings;
            _internetTime = internetTime;
            _rewardAvailableTrigger = rewardAvailableTrigger;
            _serverTimeReceivedSignal = timeReceivedSignal;
            _serverTimeReceivedSignal.Event += CheckForReward;
        }

        public bool IsRewardExists()
        {
            if (!_session.IsGameStarted || System.DateTime.UtcNow.Ticks - _session.Game.GameStartTime  < System.TimeSpan.TicksPerDay)
                return false;

            return _internetTime.HasBeenReceived && IsRewardExists(TimeToDays(_internetTime.DateTime));
        }

        public IEnumerable<IProduct> CollectReward()
        {
            var size = GetRewardSizeAndUpdate();

            if (size <= 0)
                return null;

            var level = StarLayout.GetStarLevel(_session.StarMap.FurthestVisitedStar, 0);
            var seed = TimeToDays(_internetTime.DateTime);
            return _lootGenerator.GetDailyReward(size, level, seed);
        }

        private void CheckForReward(DateTime time)
        {
            if (!_session.IsGameStarted)
                return;

            var days = TimeToDays(time);
            if (_session.Social.LastDailyRewardDate > days)
            {
                _session.Social.LastDailyRewardDate = days;
                _session.Social.FirstDailyRewardDate = 0;
            }
            if (_gameSettings.LastDailyRewardDate > days)
            {
                _gameSettings.LastDailyRewardDate = days;
            }
            
            if (IsRewardExists(days))
                _rewardAvailableTrigger.Fire();
        }

        private bool IsRewardExists(int currentDate)
        {
            var lastDate = Mathf.Max(_session.Social.LastDailyRewardDate, _gameSettings.LastDailyRewardDate);
            var daysLeft = currentDate - lastDate;

            return daysLeft > 0;
        }

        private int GetRewardSizeAndUpdate()
        {
            if (!_internetTime.HasBeenReceived)
                return 0;

            var currentDate = TimeToDays(_internetTime.DateTime);
            var lastDate = Mathf.Max(_session.Social.LastDailyRewardDate, _gameSettings.LastDailyRewardDate);
            var daysLeft = currentDate - lastDate;

            if (daysLeft < 0)
                return 0;

            _session.Social.LastDailyRewardDate = currentDate;
            _gameSettings.LastDailyRewardDate = currentDate;

            if (daysLeft != 1 || _session.Social.FirstDailyRewardDate == 0)
            {
                _session.Social.FirstDailyRewardDate = currentDate;
                return 1;
            }

            return 1 + currentDate - _session.Social.FirstDailyRewardDate;
        }

        public static int TimeToDays(DateTime time)
        {
            return (int)(time.Ticks / TimeSpan.TicksPerDay);
        }

        protected override void OnSessionDataLoaded()
        {
        }

        protected override void OnSessionCreated()
        {
            if (_internetTime.HasBeenReceived)
                CheckForReward(_internetTime.DateTime);
        }

        private readonly ISessionData _session;
        private readonly GameSettings _gameSettings;
        private readonly InternetTimeService _internetTime;
        private readonly ServerTimeReceivedSignal _serverTimeReceivedSignal;
        private readonly DailyRewardAwailableSignal.Trigger _rewardAvailableTrigger;
        private readonly LootGenerator _lootGenerator;
    }

    public class DailyRewardAwailableSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase {}
    }
}
