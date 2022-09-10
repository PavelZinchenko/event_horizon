using System.Collections.Generic;
using System.Linq;
using Constructor;
using GameServices.GameManager;
using Session;
using Session.Content;
using Zenject;
using Constructor.Ships;
using Database.Legacy;
using Diagnostics;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine.Assertions;
using Utils;

namespace GameServices.Player
{
    public sealed class PlayerFleet : GameServiceBase
    {
        [Inject]
        public PlayerFleet(
            ISessionData session, 
            SessionDataLoadedSignal dataLoadedSignal, 
            SessionCreatedSignal sessionCreatedSignal, 
            SessionAboutToSaveSignal sessionAboutToSaveSignal,
            IDebugManager debugManager,
            PlayerSkillsResetSignal playerSkillsResetSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
            _session = session;
            _debugManager = debugManager;

            _sessionAboutToSaveSignal = sessionAboutToSaveSignal;
            _sessionAboutToSaveSignal.Event += OnSessionAboutToSave;
            _playerSkillsResetSignal = playerSkillsResetSignal;
            _playerSkillsResetSignal.Event += OnPlayerSkillsReset;

            _ships.ItemAddedEvent += OnShipAdded;
            _ships.ItemRemovedEvent += OnShipRemoved;
            _ships.EntireCollectionChangedEvent += OnShipCollectionChanged;
        }

        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly PlayerInventory _inventory;
        [Inject] private readonly IDatabase _database;

        public ShipSquad ActiveShipGroup => _activeShips;

        public IShip ExplorationShip
        {
            get => _explorationShip;
            set
            {
                Assert.IsNotNull(value);
                Assert.IsTrue(value.Model.SizeClass == SizeClass.Frigate);
                Assert.IsTrue(_ships.Contains(value));
                _explorationShip = value;
                DataChanged = true;
            }
        }

        public IEnumerable<IShip> GetAllHangarShips()
        {
            return _activeShips.Ships;
        }

        public ObservableCollection<IShip> Ships => _ships;

        private void OnShipCollectionChanged()
        {
            DataChanged = true;
        }

        private void OnShipAdded(IShip ship)
        {
            DataChanged = true;
            _session.Statistics.UnlockShip(ship.Id);
        }

        private void OnShipRemoved(IShip ship)
        {
            DataChanged = true;
            _activeShips.Remove(ship);
            //foreach (var group in _shipGroups)
            //    group.Remove(ship);
        }

        public float Power { get { return ActiveShipGroup.Ships.Sum(ship => Maths.Threat.GetShipPower(ship)); } }

        protected override void OnSessionDataLoaded()
        {
            Load();
        }

        protected override void OnSessionCreated()
        {
            var components = new List<IntegratedComponent>();
            foreach (var ship in _ships)
            {
                var debug = _debugManager.CreateLog(ship.Name);
                CheckShipComponents(ship.Model.Layout, ship.Model.Barrels, ship.Components, components, debug);
                ship.Components.Assign(components);

                if (ship.FirstSatellite != null)
                {
                    CheckShipComponents(ship.FirstSatellite.Information.Layout, ship.FirstSatellite.Information.Barrels, ship.FirstSatellite.Components, components, debug);
                    ship.FirstSatellite.Components.Assign(components);
                }

                if (ship.SecondSatellite != null)
                {
                    CheckShipComponents(ship.SecondSatellite.Information.Layout, ship.SecondSatellite.Information.Barrels, ship.SecondSatellite.Components, components, debug);
                    ship.SecondSatellite.Components.Assign(components);
                }
            }
            
            foreach (var data in _strippedComponents)
            {
                _inventory.Components.Add(data.Key, data.Value);
            }
            _strippedComponents.Clear();

            _activeShips.CheckIfValid(_playerSkills, true);
        }

        private void CheckShipComponents(Layout layoutData, IEnumerable<Barrel> barrels, IEnumerable<IntegratedComponent> components, IList<IntegratedComponent> validComponents, IDebugLog debugLog)
        {
            var layout = new Constructor.ShipLayout(layoutData, barrels, Enumerable.Empty<Constructor.IntegratedComponent>(), debugLog);
            validComponents.Clear();
            var random = new System.Random(_session.Game.Seed);

            foreach (var item in components)
            {
                IntegratedComponent component = item;
                if (!component.Info)
                    continue;

                if (!item.Info.IsValidModification)
                {
                    component = new IntegratedComponent(ComponentInfo.CreateRandomModification(item.Info.Data, random, item.Info.ModificationQuality, item.Info.ModificationQuality), item.X, item.Y, 
                        item.BarrelId, item.KeyBinding, item.Behaviour, item.Locked);
                }

                var id = layout.InstallComponent(component.Info, component.X, component.Y);
                if (id >= 0)
                    validComponents.Add(component);
                else
                    _inventory.Components.Add(component.Info);
            }
        }

        private void OnPlayerSkillsReset()
        {
            _activeShips.CheckIfValid(_playerSkills, true);
        }

        private void Load()
        {
            Clear();

            var ships = new List<IShip>();
            foreach (var item in _session.Fleet.Ships)
            {
                bool added = false;
                try
                {
                    var ship = ShipExtensions.FromShipData(_database, item);
                    added = ship != null;
                    ships.Add(ship);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    ships.Add(null);
                    added = false;
                    UnityEngine.Debug.Log("Unknown ship: " + item.Id);
                }

                if (!added)
                {
                    foreach (var component in item.Components.FromShipComponentsData(_database))
                    {
                        // Locked components are not stripped
                        if (component.Locked) continue;
                        var info = component.Info;
                        if (!_strippedComponents.TryGetValue(info, out var amount)) _strippedComponents[info] = 1;
                        else _strippedComponents[info] = amount + 1;
                    }
                }
            }

            if (ships.Count == 0)
            {
                CreateDefault();
                return;
            }

            _ships.Assign(ships.Where(ship => ship != null));

            _activeShips.Clear();
            foreach (var item in _session.Fleet.Hangar)
            {
                UnityEngine.Debug.Log("group:" + item.Index + " ship:" + item.ShipId);
                _activeShips[item.Index] = ships[item.ShipId];
            }

            _explorationShip = _session.Fleet.ExplorationShipId >= 0 ? ships[_session.Fleet.ExplorationShipId] : null;

            UnityEngine.Debug.Log("PlayerFleet.Load: " + _ships.Count + " ships");

            DataChanged = false;
        }

        private void SaveShips()
        {
            UnityEngine.Debug.Log("PlayerFleet.SaveShips - " + _ships.Count);

            _session.Fleet.Ships.Clear();

            var shipIndices = new Dictionary<IShip, int>();
            var index = 0;
            foreach (var ship in _ships)
            {
                var info = ship.ToShipData();
                _session.Fleet.Ships.Add(info);
                shipIndices.Add(ship, index++);
            }

            _session.Fleet.Hangar.Clear();
            for (var j = 0; j < _activeShips.Count; ++j)
            {
                var ship = _activeShips[j];
                if (ship == null)
                    continue;

                if (!shipIndices.TryGetValue(ship, out var id))
                    continue;

                _session.Fleet.Hangar.Add(new FleetData.HangarSlotInfo { ShipId = id, Index = j });
            }

            if (_explorationShip != null && shipIndices.TryGetValue(_explorationShip, out var explorationShipId))
                _session.Fleet.ExplorationShipId = explorationShipId;
            else
                _session.Fleet.ExplorationShipId = -1;

            DataChanged = false;
        }

        private void CreateDefault()
        {
            Clear();

            var startingBuilds = _database.GalaxySettings.StartingShipBuilds;
            if (startingBuilds.Count == 0)
            {
                startingBuilds += new[] {
                    _database.GetShipBuild(new ItemId<ShipBuild>(39)),
                    _database.GetShipBuild(new ItemId<ShipBuild>(45)),
                    _database.GetShipBuild(new ItemId<ShipBuild>(51)),
                };
            }

            foreach (var build in startingBuilds)
            {
                var ship = new CommonShip(build);
                _ships.Add(ship);
                ActiveShipGroup.Add(ship);
            }

            _explorationShip = _ships.FirstOrDefault(ship => ship.Model.SizeClass == SizeClass.Frigate);

            foreach (var ship in _ships)
                foreach (var item in ship.Components)
                    item.Locked = false;
        }

        private void OnSessionAboutToSave()
        {
            UnityEngine.Debug.Log("PlayerFleet.OnSessionAboutToSave");

            if (DataChanged)
                SaveShips();
        }

        private void Clear()
        {
            _ships.Clear();
            _activeShips.Clear();
            _strippedComponents.Clear();
        }

        private bool DataChanged
        {
            get
            {
                return _dataChanged || _activeShips.IsChanged || _ships.Any(ship => ship.DataChanged);
            }
            set
            {
                _dataChanged = value;
                if (_dataChanged)
                    return;

                _activeShips.IsChanged = false;

                foreach (var ship in _ships)
                    ship.DataChanged = false;
            }
        }

        private bool _dataChanged;
        private IShip _explorationShip;
        private readonly ShipSquad _activeShips = new ShipSquad();
        private readonly ObservableCollection<IShip> _ships = new ObservableCollection<IShip>();

        private readonly Dictionary<ComponentInfo, int> _strippedComponents = new Dictionary<ComponentInfo, int>();
        private readonly ISessionData _session;
        private readonly IDebugManager _debugManager;
        private readonly SessionAboutToSaveSignal _sessionAboutToSaveSignal;
        private readonly PlayerSkillsResetSignal _playerSkillsResetSignal;
    }
}
