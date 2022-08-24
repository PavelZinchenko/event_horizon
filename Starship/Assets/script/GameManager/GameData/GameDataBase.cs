//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GameModel.Serialization;
//using Zenject;

// TODO: delete
//namespace GameModel.GameData
//{
//    public class GameDataBase : Services.Storage.IGameData
//	{
//        public GameDataBase()
//        {
//            Reset();
//        }

//        public void Reset()
//        {
//            GameId = System.DateTime.UtcNow.Ticks;
//            TimePlayed = 0;

//            _game = new GameData();
//            _starmap = new StarMapData();
//            _player = new PlayerData();
//            _shop = new ShopData();
//            _events = new EventData();
//            _bosses = new BossData();
//            _regions = new RegionData();
//            _wormholes = new WormholeData();
//            _commonObjectData = new CommonObjectData();
//            _researchData = new ResearchData();
//            _statisticsData = new StatisticsData();
//            _resourcesData = new ResourcesData();
//            _upgradesData = new UpgradesData();
//            _purchases = new InAppPurchasesData();
//            _achievements = new AchievementData();
//        }

//        public static GameDataBase TryLoad(Services.Storage.IDataStorage dataStorage)
//        {
//            if (dataStorage == null)
//                return null;
//            var storage = new GameDataBase();
//            return dataStorage.TryLoad(storage) ? storage : null;
//        }

//        public static GameDataBase TryLoadLegacyGame()
//        {
//            var storage = new GameDataBase();

//            var game = GameData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Game));
//            if (game == null)
//                return null;

//            storage._game = game;
//            storage._starmap = StarMapData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.StarMap)) ?? new StarMapData();
//            storage._player = PlayerData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Player)) ?? new PlayerData();
//            storage._shop = ShopData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Shop)) ?? new ShopData();
//            storage._events = EventData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Events)) ?? new EventData();
//            storage._bosses = BossData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Boss)) ?? new BossData();
//            storage._regions = RegionData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Region)) ?? new RegionData();
//            storage._purchases = InAppPurchasesData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Purchases)) ?? new InAppPurchasesData();
//            storage._wormholes = WormholeData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Wormhole)) ?? new WormholeData();
//            storage._achievements = AchievementData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Achievement)) ?? new AchievementData();
//            storage._commonObjectData = CommonObjectData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Laboratory)) ?? new CommonObjectData();
//            storage._researchData = ResearchData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Research)) ?? new ResearchData();
//            storage._statisticsData = StatisticsData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Craft)) ?? new StatisticsData();
//            storage._resourcesData = ResourcesData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Resources)) ?? new ResourcesData();
//            storage._upgradesData = UpgradesData.Deserialize(LegacyDataStorage.ReadData(LegacyDataStorage.GameDataType.Upgrades)) ?? new UpgradesData();

//            storage.GameId = game.GameStartTime;
//            storage.TimePlayed = 0;

//            return storage;
//        }

//        public long GameId { get; private set; }
//        public long TimePlayed { get; private set; }

//        public long DataVersion
//        {
//            get
//            {
//                return IsChanged ? _version + 1 : _version;
//            }
//            private set
//            {
//                _version = value;
//            }
//        }

//        public void MergePurchases(GameDataBase other)
//        {
//            Purchases.RemoveAds |= other.Purchases.RemoveAds;
//            Purchases.SupporterPack |= other.Purchases.SupporterPack;
//            var stars = other.Purchases.PurchasedStars - Purchases.PurchasedStars;
//            if (stars > 0)
//            {
//                Purchases.PurchasedStars += stars;
//                Resources.Stars += stars;
//            }
//        }

//        public IEnumerable<byte> Serialize()
//        {
//            if (IsChanged)
//            {
//                #if UNITY_EDITOR
//                foreach (var item in Data)
//                    if (item.IsChanged)
//                        UnityEngine.Debug.Log("IsChanged: " + item.FileName);
//                #endif
//                _version++;
//            }
            
//            foreach (var value in Helpers.Serialize(1)) // formatId
//                yield return value;

//            foreach (var value in Helpers.Serialize(Data.Count()))
//                yield return value;

//            foreach (var item in Data)
//            {
//                foreach (var value in Helpers.Serialize(item.FileName))
//                    yield return value;
//                var buffer = item.Serialize().ToArray();
//                foreach (var value in Helpers.Serialize(buffer.Length))
//                    yield return value;
//                foreach (var value in buffer)
//                    yield return value;
//            }
//        }

//        public bool TryDeserialize(long gameId, long timePlayed, long dataVersion, byte[] data, int startIndex)
//        {
//            int index = startIndex;
//            var formatId = Helpers.DeserializeInt(data, ref index);
//            if (formatId != 1)
//                return false;

//            GameId = gameId;
//            TimePlayed = timePlayed;
//            DataVersion = dataVersion;

//            var count = Helpers.DeserializeInt(data, ref index);
//            for (var i = 0; i < count; ++i)
//            {
//                var id = Helpers.DeserializeString(data, ref index);

//                id = TryConvertId(id); // TODO: remove after release

//                if (id == GameData.Name)
//                    _game = GameData.Deserialize(GetSubArray(data, ref index));
//                else if (id == StarMapData.Name)
//                    _starmap = StarMapData.Deserialize(GetSubArray(data, ref index));
//                else if (id == PlayerData.Name)
//                    _player = PlayerData.Deserialize(GetSubArray(data, ref index));
//                else if (id == ShopData.Name)
//                    _shop = ShopData.Deserialize(GetSubArray(data, ref index));
//                else if (id == EventData.Name)
//                    _events = EventData.Deserialize(GetSubArray(data, ref index));
//                else if (id == BossData.Name)
//                    _bosses = BossData.Deserialize(GetSubArray(data, ref index));
//                else if (id == RegionData.Name)
//                    _regions = RegionData.Deserialize(GetSubArray(data, ref index));
//                else if (id == WormholeData.Name)
//                    _wormholes = WormholeData.Deserialize(GetSubArray(data, ref index));
//                else if (id == CommonObjectData.Name)
//                    _commonObjectData = CommonObjectData.Deserialize(GetSubArray(data, ref index));
//                else if (id == ResearchData.Name)
//                    _researchData = ResearchData.Deserialize(GetSubArray(data, ref index));
//                else if (id == StatisticsData.Name)
//                    _statisticsData = StatisticsData.Deserialize(GetSubArray(data, ref index));
//                else if (id == ResourcesData.Name)
//                    _resourcesData = ResourcesData.Deserialize(GetSubArray(data, ref index));
//                else if (id == InAppPurchasesData.Name)
//                    _purchases = InAppPurchasesData.Deserialize(GetSubArray(data, ref index));
//                else if (id == AchievementData.Name)
//                    _achievements = AchievementData.Deserialize(GetSubArray(data, ref index));
//                else if (id == UpgradesData.Name)
//                    _upgradesData = UpgradesData.Deserialize(GetSubArray(data, ref index));

//				// TODO: delete in 0.12
//				if (_upgradesData.DeprecatedUpgradesPrice > 0)
//					_resourcesData.Stars += _upgradesData.DeprecatedUpgradesPrice;
//            }

//            return !Data.Contains(null);
//        }

//#if UNITY_STANDALONE
//        private static string TryConvertId(string id)
//        {
//            switch (id)
//            {
//                case "2333233223":
//                    return GameData.Name;
//                case "2232222323":
//                    return StarMapData.Name;
//                case "2232333223":
//                    return PlayerData.Name;
//                case "2322222323":
//                    return ShopData.Name;
//                case "3233233223":
//                    return EventData.Name;
//                case "2223233223":
//                    return BossData.Name;
//                case "2323333223":
//                    return RegionData.Name;
//                case "3332222323":
//                    return WormholeData.Name;
//                case "2323233223":
//                    return CommonObjectData.Name;
//                case "3323333223":
//                    return ResearchData.Name;
//                case "3323233223":
//                    return StatisticsData.Name;
//                case "2233333223":
//                    return ResourcesData.Name;
//                case "3222333223":
//                    return InAppPurchasesData.Name;
//                case "2332233223":
//                    return AchievementData.Name;
//                case "2332222323":
//                    return UpgradesData.Name;
//            }

//            return id;
//        }
//#else
//        private static string TryConvertId(string id)
//        {
//            switch (id)
//            {
//                case "2333233223":
//                    return GameData.Name;
//                case "2232222323":
//                    return StarMapData.Name;
//                case "2232333223":
//                    return PlayerData.Name;
//                case "2322222323":
//                    return ShopData.Name;
//                case "3233233223":
//                    return EventData.Name;
//                case "2223233223":
//                    return BossData.Name;
//                case "2323333223":
//                    return RegionData.Name;
//                case "3332222323":
//                    return WormholeData.Name;
//                case "2323233223":
//                    return CommonObjectData.Name;
//                case "3323333223":
//                    return ResearchData.Name;
//                case "3323233223":
//                    return StatisticsData.Name;
//                case "2233333223":
//                    return ResourcesData.Name;
//                case "3222333223":
//                    return InAppPurchasesData.Name;
//                case "2332233223":
//                    return AchievementData.Name;
//                case "2332222323":
//                    return UpgradesData.Name;
//            }

//            return id;
//        }
//#endif

//        public GameData Game { get { return _game; } }
//        public StarMapData StarMap { get { return _starmap; } }
//        public PlayerData Player { get { return _player; } }
//        public ShopData Shop { get { return _shop; } }
//        public EventData Events { get { return _events; } }
//        public BossData Bosses { get { return _bosses; } }
//        public RegionData Regions { get { return _regions; } }			
//        public InAppPurchasesData Purchases { get { return _purchases; } }
//        public WormholeData Wormholes { get { return _wormholes; } }			
//        public AchievementData Achievements { get { return _achievements; } }
//        public CommonObjectData CommonObjects { get { return _commonObjectData; } }
//        public ResearchData Research { get { return _researchData; } }
//        public StatisticsData Statistics { get { return _statisticsData; } }
//        public ResourcesData Resources { get { return _resourcesData; } }
//        public UpgradesData Upgrades { get { return _upgradesData; } }
		
////#if UNITY_EDITOR
////		public static void LoadDataFromSingleFile()
////		{
////			UnityEngine.Debug.Log("LoadDataFromSingleFile");
////
////			byte[] data = null;
////			try { data = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/cloudsave.dat"); }
////			catch(Exception e) { UnityEngine.Debug.Log(e.Message); }
////
////			if (data != null)
////				SetGameData(data);
////		}
////#endif

//        public static GameDataBase TryConvertFromLegacyGameData(byte[] data)
//        {
//            var storage = new GameDataBase();

//            int index = 0;
//            var version = Helpers.DeserializeInt(data, ref index);
//            if (version > 1)
//                return null;

//            var game = GameData.Deserialize(GetSubArray(data, ref index));
//            if (game == null)
//                return null;

//            storage._game = game;
//            storage._starmap = StarMapData.Deserialize(GetSubArray(data, ref index)) ?? new StarMapData();
//            storage._player = PlayerData.Deserialize(GetSubArray(data, ref index)) ?? new PlayerData();
//            storage._shop = ShopData.Deserialize(GetSubArray(data, ref index)) ?? new ShopData();
//            storage._events = EventData.Deserialize(GetSubArray(data, ref index)) ?? new EventData();
//            storage._bosses = BossData.Deserialize(GetSubArray(data, ref index)) ?? new BossData();
//            storage._regions = RegionData.Deserialize(GetSubArray(data, ref index)) ?? new RegionData();
//            storage._wormholes = WormholeData.Deserialize(GetSubArray(data, ref index)) ?? new WormholeData();
//            storage._commonObjectData = CommonObjectData.Deserialize(GetSubArray(data, ref index)) ?? new CommonObjectData();
//            storage._researchData = ResearchData.Deserialize(GetSubArray(data, ref index)) ?? new ResearchData();
//            storage._statisticsData = StatisticsData.Deserialize(GetSubArray(data, ref index)) ?? new StatisticsData();
//            storage._resourcesData = ResourcesData.Deserialize(GetSubArray(data, ref index)) ?? new ResourcesData();
//            storage._upgradesData = UpgradesData.Deserialize(GetSubArray(data, ref index)) ?? new UpgradesData();

//            storage.GameId = storage._game.GameStartTime;
//            storage.TimePlayed = System.DateTime.UtcNow.Ticks - storage._game.GameStartTime;

//            return storage;
//        }

//		private static byte[] GetSubArray(byte[] data, ref int index)
//		{
//			if (index >= data.Length)
//				return new byte[0];

//			int size = Helpers.DeserializeInt(data, ref index);
//			var newData = new byte[size];
//			Array.Copy(data, index, newData, 0, size);
//			index += size;
//			return newData;
//		}

//        private IEnumerable<ISerializableData> Data
//        {
//            get
//            {
//                yield return _game;
//                yield return _starmap;
//                yield return _player;
//                yield return _shop;
//                yield return _events;
//                yield return _bosses;
//                yield return _regions;
//                yield return _purchases;
//                yield return _wormholes;
//                yield return _achievements;
//                yield return _commonObjectData;
//                yield return _researchData;
//                yield return _statisticsData;
//                yield return _resourcesData;
//                yield return _upgradesData;
//            }
//        }

//        private bool IsChanged { get { return Data.Any(item => item.IsChanged); } }

//		private GameData _game;
//		private StarMapData _starmap;
//		private PlayerData _player;
//		private ShopData _shop;
//		private EventData _events;
//		private BossData _bosses;
//		private RegionData _regions;
//		private InAppPurchasesData _purchases;
//		private WormholeData _wormholes;
//		private AchievementData _achievements;
//		private CommonObjectData _commonObjectData;
//		private ResearchData _researchData;
//		private StatisticsData _statisticsData;
//		private ResourcesData _resourcesData;
//		private UpgradesData _upgradesData;
       
//        private long _version;
//    }
//}
