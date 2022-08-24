using System.Collections.Generic;
using Combat.Component.Unit.Classification;
using Combat.Unit;
using Constructor.Ships;
using GameDatabase;
using GameServices.Player;

namespace Combat.Domain
{
    public class FleetModel : IFleetModel
    {
        public FleetModel(IEnumerable<IShip> ships, UnitSide unitSide, IDatabase database, int level, PlayerSkills playerSkills = null)
        {
            var settings = database.ShipSettings;
            Level = level;

            foreach (var ship in ships)
            {
                var shipSpec = playerSkills != null ? ship.BuildSpecAndApplySkills(playerSkills, settings) : ship.CreateBuilder().Build(settings);
                var shipInfo = new ShipInfo(ship, shipSpec, unitSide);
                _ships.Add(shipInfo);
            }
        }

        public int Level { get; private set; }

        public IList<IShipInfo> Ships { get { return _ships.AsReadOnly(); } }

        public long GetExpForAllShips()
        {
            long experience = 0;

            foreach (var item in _ships)
            {
                var model = item.ShipUnit;
                if (model == null)
                    continue;

                var damage = model.State != UnitState.Destroyed ? 1.0f - model.Stats.Armor.Percentage : 1.0f;
                experience += (long)(damage * Maths.Experience.TotalExpForShip(item.ShipData));
            }

            return experience;
        }

        private readonly List<IShipInfo> _ships = new List<IShipInfo>();
    }
}

//        public class Fleet
//        {
//            public Fleet(ControllerType controller, Model.OwnerType owner, IEnumerable<IShip> ships, IRemoteClient remoteClient, int seed)
//            {
//                Owner = owner;
//                _ships = new ObservableCollection<IShip>(ships);
//                Controller = controller;
//                _seed = seed;
//                AiLevel = 100;
//                _remoteClient = remoteClient;
//            }

//            public ControllerType Controller { get; private set; }

//            public IList<IShip> Ships { get { return _ships.AsReadOnly(); } }
//            public KeyValuePair<IShip, IShipCombatModel> ActiveShip { get { return _activeShips.FirstOrDefault(item => item.Value.IsActiveObject); } }
//            public IShip GetShipByModel(IShipCombatModel model) { return _activeShips.FirstOrDefault(item => item.Value == model).Key; }
//            public IShip LastActivatedShip { get; private set; }

//            public Model.OwnerType Owner { get; private set; }
//            public int AiLevel { get; set; }

//            public IShip AnyShip { get { return _ships.FirstOrDefault(ship => GetCondition(ship) > 0); } }
//            public IShip AnyInactiveShip { get { return _ships.FirstOrDefault(ship => !_activeShips.ContainsKey(ship)); } }
//            public bool HasMoreShips { get { return _activeShips.Count < _ships.Count; } }
//            public bool IsEmpty { get { return AnyShip == null; } }
//            public bool UseBonuses { get; set; }
//            public bool DisableExperience { get; set; }

//            public void AddExperience(IShip ship, long value)
//            {
//                long experience;
//                if (!_experience.TryGetValue(ship, out experience))
//                    experience = 0;

//                _experience[ship] = experience + value;
//            }

//            public long GetExperience(IShip ship)
//            {
//                long experience;
//                return _experience.TryGetValue(ship, out experience) ? experience : 0;
//            }

//            public int AvailableShipCount
//            {
//                get
//                {
//                    var activeShipCount = _activeShips.Values.Count(item => item.IsDestroyed() || item.IsActiveObject);
//                    return _ships.Count - activeShipCount;
//                }
//            }

//            public int ActiveShipCount { get { return _activeShips.Values.Count(item => item.IsActiveObject); } }

//            public long GetTotalExperience()
//            {
//                long experience = 0;

//                if (DisableExperience)
//                    return experience;

//                foreach (var item in _activeShips)
//                {
//                    var model = item.Value;
//                    if (model == null)
//                        continue;

//                    var damage = model.IsDestroyed() ? 1.0f : 1.0f - model.ArmorPoints.Percentage;
//                    experience += (long)(damage * Maths.Experience.TotalExpForShip(model));
//                }

//                return experience;
//            }

//            public long GetTotalResearchPoints()
//            {
//                long experience = 0;
//                foreach (var item in _activeShips)
//                {
//                    var model = item.Value;
//                    if (model == null)
//                        continue;

//                    var damage = model.IsDestroyed() ? 1.0f : 1.0f - model.ArmorPoints.Percentage;
//                    experience += (long)(damage * Maths.Research.TotalExpForShip(model));
//                }

//                return experience;
//            }


//            public float ZOrder { get; set; }

//            public IShipCombatModel ActivateShip(IShip ship, Position position, float rotation, bool showDamage, PlayerSkills playerSkills, IMessenger messenger, FactoryContext context, IAiManager aiManager, IDatabase database)
//            {
//                //UnityEngine.Debug.Log("CombatData.ActivateShip");
//                IShipCombatModel model = null;
//                if (_activeShips.TryGetValue(ship, out model) && model.IsActiveObject)
//                {
//                    //UnityEngine.Debug.Log("CombatData.ActivateShip - already exists");
//                    return model;
//                }

//                var armorPoints = model == null ? 1.0f : model.ArmorPoints.Percentage;
//                var hullPoints = model == null ? 1.0f : model.HullPoints.Percentage;

//                LastActivatedShip = ship;

//                model = CreateShipModel(ref ship, position, rotation, armorPoints, hullPoints, showDamage, playerSkills, context, aiManager, database);

//                _activeShips[ship] = model;

//                //UnityEngine.Debug.Log("CombatData.ActivateShip - broadcasting");
//                messenger.Broadcast(EventType.CombatShipCreated);
//                //UnityEngine.Debug.Log("CombatData.ActivateShip - done");
//                return model;
//            }

//            public float GetCondition(IShip ship)
//            {
//                IShipCombatModel model = null;
//                if (_activeShips.TryGetValue(ship, out model))
//                    return model.Life;
//                return 1.0f;
//            }

//            private IShipCombatModel CreateShipModel(ref IShip ship, Position position, float rotation, float armorPointsPercentage, float hullPointsPercentage, bool showDamage, PlayerSkills playerSkills, FactoryContext context, IAiManager aiManager, IDatabase database)
//            {
//                if (ShouldCreatePirateShip)
//                {
//                    ship = new EnemyShip(database.GetShipBuild(LegacyShipBuildNames.GetId(EncryptedStrings.invaderPirate.Decrypt()))) { Experience = new Maths.Experience(Maths.Experience.FromLevel(500), false) };
//                    return new ShipFactory(context, aiManager, database).CreateShip(ship.CreateBuilder().Build(database.ShipBuilderSettings), Controller, position, rotation, _seed, ZOrder, 100, Owner, 1000, showDamage);
//                }

//                var spec = UseBonuses ? ship.BuildSpecAndApplySkills(playerSkills, database.ShipBuilderSettings) : ship.CreateBuilder().Build(database.ShipBuilderSettings);

//                IShipCombatModel model;
//                var factory = new ShipFactory(context, aiManager, database);

//                if (ship.Model.Category == ShipCategory.Starbase)
//                {
//                    var color = UnityEngine.Color.Lerp(UnityEngine.Color.white, ColorTable.FactionColor(ship.Model.Faction), 0.5f);
//                    model = factory.CreateStarbase(spec, position, rotation, _seed, ZOrder, AiLevel, Owner, ship.Experience.Level, showDamage, color);
//                }
//                else
//                    model = factory.CreateShip(spec, Controller, position, rotation, _seed, ZOrder, AiLevel, Owner,
//                        ship.Experience.Level, showDamage, _remoteClient);

//                model.ArmorPoints.Get((1f - armorPointsPercentage) * model.ArmorPoints.MaxValue);
//                model.HullPoints.Get((1f - hullPointsPercentage) * model.HullPoints.MaxValue);
//                //UnityEngine.Debug.Log("CombatData.CreateShipModel - done");
//                return model;
//            }

//            private bool ShouldCreatePirateShip
//            {
//                get
//                {
//                    if (Controller.IsPlayer())
//                        return false;
//                    if (_activeShips.Count == 0)
//                        return false;
//                    return !CertificateChecker.IsValid;
//                }
//            }

//            private readonly int _seed;
//            private readonly ObservableCollection<IShip> _ships;
//            private readonly Dictionary<IShip, long> _experience = new Dictionary<IShip, long>();
//            private readonly Dictionary<IShip, IShipCombatModel> _activeShips = new Dictionary<IShip, IShipCombatModel>();
//            private readonly IRemoteClient _remoteClient;
//        }
