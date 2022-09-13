﻿using GameDatabase;
using Session.Content;
using Zenject;

namespace Session
{
    public class ContentFactory
    {
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly StarsValueChangedSignal.Trigger _starsValueChangedTrigger;
        [Inject] private readonly FuelValueChangedSignal.Trigger _fuelValueChangedTrigger;
        [Inject] private readonly MoneyValueChangedSignal.Trigger _moneyValueChangedTrigger;
        [Inject] private readonly TokensValueChangedSignal.Trigger _tokensValueChangedTrigger;
        [Inject] private readonly PlayerPositionChangedSignal.Trigger _playerPositionChangedTrigger;
        [Inject] private readonly NewStarExploredSignal.Trigger _newStarExploredTrigger;
        [Inject] private readonly PlayerSkillsResetSignal.Trigger _playerSkillResetTrigger;
        [Inject] private readonly ResourcesChangedSignal.Trigger _specialResourcesChangedTrigger;

        public BossData CreateBossData(byte[] buffer)
        {
            return new BossData(buffer);
        }

        public CommonObjectData CreateCommonObjectData(byte[] buffer)
        {
            return new CommonObjectData(buffer);
        }

        public EventData CreateEventData(byte[] buffer)
        {
            return new EventData(buffer);
        }

        public GameData CreateGameData(byte[] buffer)
        {
            return new GameData(buffer);
        }

        public InventoryData CreateInventoryData(byte[] buffer)
        {
            return new InventoryData(buffer);
        }

        public FleetData CreateFleetData(byte[] buffer)
        {
            return new FleetData(_database, buffer);
        }

        public RegionData CreateRegionData(byte[] buffer, int gameSeed)
        {
            return new RegionData(gameSeed, buffer);
        }

        public ResearchData CreateResearchData(byte[] buffer)
        {
            return new ResearchData(buffer);
        }

        public ResourcesData CreateResourcesData(byte[] buffer)
        {
            return new ResourcesData(_fuelValueChangedTrigger, _moneyValueChangedTrigger,
                _starsValueChangedTrigger, _tokensValueChangedTrigger, _specialResourcesChangedTrigger, buffer);
        }

        public ShopData CreateShopData(byte[] buffer)
        {
            return new ShopData(buffer);
        }

        public StarMapData CreateStarMapData(byte[] buffer)
        {
            return new StarMapData(_playerPositionChangedTrigger, _newStarExploredTrigger, buffer);
        }

        public StatisticsData CreateStatisticsData(byte[] buffer)
        {
            return new StatisticsData(buffer);
        }

        public UpgradesData CreateUpgradesData(byte[] buffer)
        {
            return new UpgradesData(_playerSkillResetTrigger, buffer);
        }

        public WormholeData CreateWormholeData(byte[] buffer)
        {
            return new WormholeData(buffer);
        }

        public PvpData CreatePvpData(byte[] buffer)
        {
            return new PvpData(buffer);
        }
        
        public QuestData CreateQuestData(byte[] buffer)
        {
            return new QuestData(buffer);
        }
    }
}
