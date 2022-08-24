using Constructor;
using GameServices.Database;
using GameServices.Player;
using Maths;
using Session;
using Constructor.Ships;
using Economy.ItemType;
using GameServices.GameManager;
using GameServices.Gui;
using Services.Account;
using Services.Unity;
using UnityEngine;
using Zenject;
using Research = GameServices.Research.Research;
using Status = Services.Account.Status;
using System.Linq;
using Database.Legacy;
using Galaxy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.LevelManager;
using GameDatabase.Extensions;

public class Cheats
{
    [Inject] private readonly PlayerFleet _playerFleet;
    [Inject] private readonly PlayerInventory _playerInventory;
    [Inject] private readonly PlayerResources _playerResources;
    [Inject] private readonly PlayerSkills _playerSkills;
    [Inject] private readonly Research _research;
    [Inject] private readonly ITechnologies _technologies;
    [Inject] private readonly ISessionData _session;
    [Inject] private readonly IGameDataManager _gameDataManager;
    [Inject] private readonly SessionDataLoadedSignal.Trigger _dataLoadedTrigger;
    [Inject] private readonly SessionCreatedSignal.Trigger _sesionCreatedTrigger;
    [Inject] private readonly IAccount _account;
    [Inject] private readonly ICoroutineManager _coroutineManager;
    [Inject] private readonly GuiHelper _guiHelper;
    [Inject] private readonly ItemTypeFactory _itemTypeFactory;
    [Inject] private readonly MotherShip _motherShip;
    [Inject] private readonly StarMap _starMap;
    [Inject] private readonly IDatabase _database;
    [Inject] private readonly ILevelLoader _levelLoader;

    public bool TryExecuteCommand(string command, int hash)
	{
		#if UNITY_EDITOR
		if (command == "123")
		{
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(49))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(19))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(80))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(78))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(65))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(85))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(99))) { Experience = Maths.Experience.FromLevel(50) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(25))) { Experience = Maths.Experience.FromLevel(50) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(28))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(7))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(16))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(10))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(31))) { Experience = Maths.Experience.FromLevel(100) });
		    _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(22))) { Experience = Maths.Experience.FromLevel(100) });

            foreach (var item in _database.ComponentList.CommonAndRare())
                _playerInventory.Components.Add(new ComponentInfo(item), 25);

      //      _playerResources.Money += 200000;
      //      _playerResources.Stars += 50;

		    foreach (var faction in _database.FactionList.Visible())
			    _research.AddResearchPoints(faction, 50);

		    _playerSkills.Experience = GameModel.Skills.Experience.FromLevel(_playerSkills.Experience.Level + 50);
        }
        else if (command == "345")
        {
            _playerResources.Tokens += 1000;
        }
		else if (command == "000")
		{
			_playerResources.Fuel += 1000;
		}
        else if (command == "666")
        {
            var experience = Experience.FromLevel(200);
            for (var i = 0; i < 3; ++i)
            {
                _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader1"))) { Experience = experience });
                _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader2"))) { Experience = experience });
                _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader3"))) { Experience = experience });
                _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(LegacyShipBuildNames.GetId("MyInvader4"))) { Experience = experience });
            }
        }
        else if (command == "667")
        {
            _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(265))));
            _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(266))));
            _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(267))));
            _playerFleet.Ships.Add(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(235))));
        }
        else if (command == "999")
        {
            var random = new System.Random();
            for (var i = 0; i < 100; ++i)
            {
                _playerInventory.Components.Add(ComponentInfo.CreateRandomModification(_database.GetComponent(LegacyComponentNames.GetId("Nanofiber")), random, ModificationQuality.N3, ModificationQuality.N3));
            }
        }
        #endif

        if (command == "000")
        {
            if (_account.Status != Status.Connected)
                _guiHelper.ShowMessage("Not logged in");
            else
                _guiHelper.ShowMessage("DisplayName: " + _account.DisplayName + "\nId: " + _account.Id);
            return true;
        }

        var id = DebugCommands.Decode(command, hash);
		
		switch (id)
		{
		case 0:
		    _session.Quests.Reset();
            break;
		case 1:
            var center = _motherShip.CurrentStar.Position;
            var stars = _starMap.GetVisibleStars(center - Vector2.one * 20f, center + Vector2.one * 20f);
            foreach (var item in stars)
                item.SetVisited();
            break;
		case 2:
		    foreach (var item in _database.QuestItemList.Where(item => item.Price > 0))
		        _playerResources.AddResource(item.Id, 100000);
			break;
		case 3:
			if (_session.Resources.Stars < 0)
                _session.Resources.Stars += 100;
			break;
		case 4:
		    _playerSkills.Experience = GameModel.Skills.Experience.FromLevel(_playerSkills.Experience.Level + 20);
			break;
		case 20:
		    foreach (var item in _database.QuestItemList.Where(item => item.Price == 0))
		        _playerResources.AddResource(item.Id, 1);
			break;
		case 19:
            foreach (var item in _database.ComponentList.Common())
                _playerInventory.Components.Add(new ComponentInfo(item), 10);
            break;
		case 5:
			_gameDataManager.LoadGameFromLocalCopy();
			break;
		case 6:
			foreach (var ship in _database.ShipBuildList.Playable())
                _playerFleet.Ships.Add(new CommonShip(ship));
			break;
		case 7:
			foreach (var ship in _database.ShipBuildList.Playable().Common())
                _playerFleet.Ships.Add(new CommonShip(ship));
			break;
		case 8:
			foreach (var ship in _database.ShipBuildList.Flagships())
                _playerFleet.Ships.Add(new CommonShip(ship));
			break;
		case 9:
			foreach (var ship in _playerFleet.ActiveShipGroup.Ships)
				ship.SetLevel(ship.Experience.Level + 10);
			break;
		case 10:
		    foreach (var faction in _database.FactionList.Visible())
				_research.AddResearchPoints(faction, 100);
			break;
		case 11:
			_session.Game.Regenerate();
            _session.StarMap.Reset();
            _session.Regions.Reset();
            _dataLoadedTrigger.Fire();
            _sesionCreatedTrigger.Fire();
			break;
		case 12:
		    _playerResources.Stars += 100000000;
			break;
		case 13:
			foreach (var ship in _playerFleet.ActiveShipGroup.Ships)
				ship.SetLevel(100);
			break;
		case 14:
			_playerResources.Money += 1000000;
			break;
		case 15:
			_playerResources.Fuel += 1000;
			break;
		case 16:
			foreach (var item in _database.ComponentList.Where(item => item.Availability == Availability.None || item.Availability == Availability.Special))
                _playerInventory.Components.Add(new ComponentInfo(item), 10);
			break;
		case 17:
			foreach (var item in _database.SatelliteList)
				_playerInventory.Satellites.Add(item, 10);
			break;
		case 18:
			foreach (var ship in _database.ShipBuildList.Where(build => build.Ship.ShipCategory == ShipCategory.Drone))
			{
				var gameShip = new CommonShip(ship);
				foreach (var item in gameShip.Components)
					item.Locked = false;
                _playerFleet.Ships.Add(gameShip);
			}
			break;
		default:
			return false;
		}

		return true;
	}
}