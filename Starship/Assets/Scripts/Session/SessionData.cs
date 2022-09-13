using System;
using System.Collections.Generic;
using Session.Content;
using Utils;
using Zenject;

namespace Session
{
    public class SessionData : ISessionData, ITickable
    {
        [Inject]
        public SessionData(ContentFactory contentFactory, SessionDataLoadedSignal.Trigger dataLoadedTrigger, SessionCreatedSignal.Trigger sesionCreatedTrigger)
        {
            _contentFactory = contentFactory;
            _sessionCreatedTrigger = sesionCreatedTrigger;
            _dataLoadedTrigger = dataLoadedTrigger;
        }

        public long GameId { get; private set;  }
        public long TimePlayed { get { return (long) _timePlayed; } private set { _timePlayed = value; } }
        public string ModId { get; private set; }

        public bool IsGameStarted { get { return _content != null && Game.GameStartTime > 0; } }

        public void Tick()
        {
            if (_content != null && _content.Game.GameStartTime > 0)
                _timePlayed += UnityEngine.Time.deltaTime;
        }

        public long DataVersion
        {
            get
            {
                return _content.IsChanged ? _version + 1 : _version;
            }
            private set
            {
                _version = value;
            }
        }

        public IEnumerable<byte> Serialize()
        {
            if (_content.IsChanged)
                _version++;

            return _content.Serialize();
        }

        public bool TryDeserialize(long gameId, long timePlayed, long dataVersion, string modId, byte[] data, int startIndex)
        {
            try
            {
                UnityEngine.Debug.Log("SessionData.TryDeserialize");

                var content = DatabaseContent.TryDeserialize(data, startIndex, _contentFactory);
                if (content == null)
                    return false;

                UnityEngine.Debug.Log("SessionData.TryDeserialize - success " + content.Game.GameStartTime + "/" + dataVersion);

                GameId = gameId;
                TimePlayed = timePlayed;
                DataVersion = dataVersion;
                ModId = modId;
                _content = content;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                return false;
            }

            try
            {
                _dataLoadedTrigger.Fire();
                _sessionCreatedTrigger.Fire();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }

            return true;
        }

        public void CreateNewGame(string modId, bool keepPurchases = true)
        {
            UnityEngine.Debug.Log("SessionData.CreateNewGame");

            _content = new DatabaseContent(_contentFactory);

            GameId = System.DateTime.UtcNow.Ticks;
            TimePlayed = 0;
            DataVersion = 0;
            ModId = modId;

            _dataLoadedTrigger.Fire();
            _sessionCreatedTrigger.Fire();
        }

        public GameData Game { get { return _content.Game; } }
        public StarMapData StarMap { get { return _content.Starmap; } }
        public InventoryData Inventory { get { return _content.Inventory; } }
        public FleetData Fleet { get { return _content.Fleet; } }
        public ShopData Shop { get { return _content.Shop; } }
        public EventData Events { get { return _content.Events; } }
        public BossData Bosses { get { return _content.Bosses; } }
        public RegionData Regions { get { return _content.Regions; } }
        public WormholeData Wormholes { get { return _content.Wormholes; } }
        public CommonObjectData CommonObjects { get { return _content.CommonObjects; } }
        public ResearchData Research { get { return _content.Research; } }
        public StatisticsData Statistics { get { return _content.Statistics; } }
        public ResourcesData Resources { get { return _content.Resources; } }
        public UpgradesData Upgrades { get { return _content.Upgrades; } }
        public PvpData Pvp { get { return _content.Pvp; } }
        public QuestData Quests { get { return _content.Quests; } }

        private long _version;
        private double _timePlayed;

        private DatabaseContent _content;
        private readonly ContentFactory _contentFactory;
        private readonly SessionDataLoadedSignal.Trigger _dataLoadedTrigger;
        private readonly SessionCreatedSignal.Trigger _sessionCreatedTrigger;
    }

    public class SessionDataStub : ISessionData
    {
        public long GameId { get; private set; }
        public long TimePlayed { get; private set; }
        public long DataVersion { get; private set; }
        public string ModId { get; private set; }

        public IEnumerable<byte> Serialize()
        {
            throw new NotSupportedException();
        }

        public bool TryDeserialize(long gameId, long timePlayed, long dataVersion, string modId, byte[] data, int startIndex)
        {
            throw new NotSupportedException();
        }

        public void CreateNewGame(string modId, bool keepPurchases = true)
        {
            throw new NotSupportedException();
        }

        public bool IsGameStarted { get; private set; }
        public GameData Game { get; private set; }
        public StarMapData StarMap { get; private set; }
        public InventoryData Inventory { get; private set; }
        public FleetData Fleet { get; private set; }
        public ShopData Shop { get; private set; }
        public EventData Events { get; private set; }
        public BossData Bosses { get; private set; }
        public RegionData Regions { get; private set; }
        public PvpData Pvp { get; private set; }
        public WormholeData Wormholes { get; private set; }
        public CommonObjectData CommonObjects { get; private set; }
        public ResearchData Research { get; private set; }
        public StatisticsData Statistics { get; private set; }
        public ResourcesData Resources { get; private set; }
        public UpgradesData Upgrades { get; private set; }
        public QuestData Quests { get; private set; }
    }

    public class SessionDataLoadedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

    public class SessionCreatedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
