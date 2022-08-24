using System;
using System.Collections.Generic;
using Session.Content;
using GameModel.Serialization;
using System.Linq;

namespace Session
{
    class DatabaseContent
    {
        public DatabaseContent(ContentFactory factory, InAppPurchasesData purchases = null, AchievementData achievements = null)
        {
            Game = factory.CreateGameData(null);
            Starmap = factory.CreateStarMapData(null);
            Inventory = factory.CreateInventoryData(null);
            Fleet = factory.CreateFleetData(null);
            Shop = factory.CreateShopData(null);
            Events = factory.CreateEventData(null);
            Bosses = factory.CreateBossData(null);
            Regions = factory.CreateRegionData(null, 0);
            Purchases = purchases ?? factory.CreateInAppPurchasesData(null);
            Wormholes = factory.CreateWormholeData(null);
            Achievements = achievements ?? factory.CreateAchievementData(null);
            CommonObjects = factory.CreateCommonObjectData(null);
            Research = factory.CreateResearchData(null);
            Statistics = factory.CreateStatisticsData(null);
            Resources = factory.CreateResourcesData(null);
            Upgrades = factory.CreateUpgradesData(null);
            Pvp = factory.CreatePvpData(null);
            Social = factory.CreateSocialData(null);
            Quests = factory.CreateQuestData(null);
            
            if (purchases != null)
                Resources.Stars = purchases.PurchasedStars;
        }

        public bool IsChanged { get { return Data.Any(item => item.IsChanged); } }

        public IEnumerable<byte> Serialize()
        {
            if (IsChanged)
            {
#if UNITY_EDITOR
                foreach (var item in Data)
                    if (item.IsChanged)
                        UnityEngine.Debug.Log("IsChanged: " + item.FileName);
#endif
                //_version++;
            }

            foreach (var value in Helpers.Serialize(1)) // formatId
                yield return value;

            foreach (var value in Helpers.Serialize(Data.Count()))
                yield return value;

            foreach (var item in Data)
            {
                foreach (var value in Helpers.Serialize(item.FileName))
                    yield return value;
                var buffer = item.Serialize().ToArray();
                foreach (var value in Helpers.Serialize(buffer.Length))
                    yield return value;
                foreach (var value in buffer)
                    yield return value;
            }
        }

        public static DatabaseContent TryDeserialize(byte[] data, int startIndex, ContentFactory factory)
        {
            var index = startIndex;
            var formatId = Helpers.DeserializeInt(data, ref index);
            if (formatId != 1)
                return null;

            var content = new DatabaseContent(factory);
            var unknownItems = new Dictionary<string, byte[]>();

            try
            {
                var count = Helpers.DeserializeInt(data, ref index);
                for (var i = 0; i < count; ++i)
                {
                    var id = Helpers.DeserializeString(data, ref index);

                    switch (id)
                    {
                    case GameData.Name:
                        content.Game = factory.CreateGameData(GetSubArray(data, ref index));
                        break;
                    case StarMapData.Name:
                        content.Starmap = factory.CreateStarMapData(GetSubArray(data, ref index));
                        break;
                    case InventoryData.Name:
                        content.Inventory = factory.CreateInventoryData(GetSubArray(data, ref index));
                        break;
                    case FleetData.Name:
                        content.Fleet = factory.CreateFleetData(GetSubArray(data, ref index));
                        break;
                    case ShopData.Name:
                        content.Shop = factory.CreateShopData(GetSubArray(data, ref index));
                        break;
                    case EventData.Name:
                        content.Events = factory.CreateEventData(GetSubArray(data, ref index));
                        break;
                    case BossData.Name:
                        content.Bosses = factory.CreateBossData(GetSubArray(data, ref index));
                        break;
                    case RegionData.Name:
                        content.Regions = factory.CreateRegionData(GetSubArray(data, ref index), content.Game.Seed);
                        break;
                    case WormholeData.Name:
                        content.Wormholes = factory.CreateWormholeData(GetSubArray(data, ref index));
                        break;
                    case CommonObjectData.Name:
                        content.CommonObjects = factory.CreateCommonObjectData(GetSubArray(data, ref index));
                        break;
                    case ResearchData.Name:
                        content.Research = factory.CreateResearchData(GetSubArray(data, ref index));
                        break;
                    case StatisticsData.Name:
                        content.Statistics = factory.CreateStatisticsData(GetSubArray(data, ref index));
                        break;
                    case ResourcesData.Name:
                        content.Resources = factory.CreateResourcesData(GetSubArray(data, ref index));
                        break;
                    case InAppPurchasesData.Name:
                        content.Purchases = factory.CreateInAppPurchasesData(GetSubArray(data, ref index));
                        break;
                    case AchievementData.Name:
                        content.Achievements = factory.CreateAchievementData(GetSubArray(data, ref index));
                        break;
                    case UpgradesData.Name:
                        content.Upgrades = factory.CreateUpgradesData(GetSubArray(data, ref index));
                        break;
                    case PvpData.Name:
                        content.Pvp = factory.CreatePvpData(GetSubArray(data, ref index));
                        break;
                    case SocialData.Name:
                        content.Social = factory.CreateSocialData(GetSubArray(data, ref index));
                        break;
                    case QuestData.Name:
                        content.Quests = factory.CreateQuestData(GetSubArray(data, ref index));
                        break;
                    default:
                        unknownItems.Add(id, GetSubArray(data, ref index));
                        break;
                    }
                }

                foreach (var item in unknownItems)
                {
                    content.DeserializeLegacyData(item.Key, item.Value, factory);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return null;
            }

            return content;
        }

        private void DeserializeLegacyData(string key, byte[] data, ContentFactory factory)
        {
            switch (key)
            {
                default:
                    UnityEngine.Debug.Log("DatabaseContent: unknown key - " + key);
                    break;
            }
        }

        public AchievementData Achievements { get; private set; }
        public GameData Game { get; private set; }
        public StarMapData Starmap { get; private set; }
        public InventoryData Inventory { get; private set; }
        public FleetData Fleet { get; private set; }
        public ShopData Shop { get; private set; }
        public EventData Events { get; private set; }
        public BossData Bosses { get; private set; }
        public RegionData Regions { get; private set; }
        public InAppPurchasesData Purchases { get; private set; }
        public WormholeData Wormholes { get; private set; }
        public CommonObjectData CommonObjects { get; private set; }
        public ResearchData Research { get; private set; }
        public StatisticsData Statistics { get; private set; }
        public ResourcesData Resources { get; private set; }
        public UpgradesData Upgrades { get; private set; }
        public PvpData Pvp { get; private set; }
        public SocialData Social { get; private set; }
        public QuestData Quests { get; private set; }

        private IEnumerable<ISerializableData> Data
        {
            get
            {
                yield return Game;
                yield return Starmap;
                yield return Inventory;
                yield return Fleet;
                yield return Shop;
                yield return Events;
                yield return Bosses;
                yield return Regions;
                yield return Purchases;
                yield return Wormholes;
                yield return Achievements;
                yield return CommonObjects;
                yield return Research;
                yield return Statistics;
                yield return Resources;
                yield return Upgrades;
                yield return Pvp;
                yield return Social;
                yield return Quests;
            }
        }

        private static byte[] GetSubArray(byte[] data, ref int index)
        {
            if (index >= data.Length)
                return new byte[0];

            int size = Helpers.DeserializeInt(data, ref index);
            var newData = new byte[size];
            Array.Copy(data, index, newData, 0, size);
            index += size;
            return newData;
        }
    }
}
