﻿using Services.Storage;
using Session.Content;

namespace Session
{
    public interface ISessionData : IGameData
    {
        bool IsGameStarted { get; }

        GameData Game { get; }
        StarMapData StarMap { get; }
        InventoryData Inventory { get; }
        FleetData Fleet { get; }
        ShopData Shop { get; }
        EventData Events { get; }
        BossData Bosses { get; }
        RegionData Regions { get; }
        PvpData Pvp { get; }
        WormholeData Wormholes { get; }
        CommonObjectData CommonObjects { get; }
        ResearchData Research { get; }
        StatisticsData Statistics { get; }
        ResourcesData Resources { get; }
        UpgradesData Upgrades { get; }
        QuestData Quests { get; }
    }
}
