using System.Collections.Generic;
using Combat.Component.Unit.Classification;
using Combat.Scene;
using Constructor.Ships;
using Economy.Products;
using GameModel.Quests;
using GameServices.Economy;
using GameServices.Player;
using Model.Military;

namespace Combat.Domain
{
    public class CombatModel : ICombatModel
    {
        public CombatModel(FleetModel playerFleet, FleetModel enemyFleet, ShipCreatedSignal shipCreatedSignal, ShipDestroyedSignal shipDestroyedSignal)
        {
            _playerFleet = playerFleet;
            _enemyFleet = enemyFleet;

            _shipCreatedSignal = shipCreatedSignal;
            _shipDestroyedSignal = shipDestroyedSignal;
            _shipCreatedSignal.Event += OnShipCreated;
            _shipDestroyedSignal.Event += OnShipDestroyed;
        }

        public CombatRules Rules { get; set; }

        public IReward GetReward(LootGenerator lootGenerator, PlayerSkills playerSkills, Galaxy.Star currentStar)
        {
            UpdateExperienceData(_playerFleet.LastActivated());

            return new CombatReward(this, playerSkills, lootGenerator, currentStar);
        }

        public IFleetModel PlayerFleet { get { return _playerFleet; } }
        public IFleetModel EnemyFleet { get { return _enemyFleet; } }

        public IEnumerable<KeyValuePair<IShip, long>> PlayerExperience { get { return _playerExperienceData; } }
        public IEnumerable<IProduct> SpecialRewards { get; set; }

        private readonly FleetModel _playerFleet;
        private readonly FleetModel _enemyFleet;

        private void OnShipCreated(Component.Ship.IShip ship) {}

        private void OnShipDestroyed(Component.Ship.IShip ship)
        {
            IShipInfo shipInfo;
            if (ship.Type.Class == UnitClass.Ship && ship.Type.Side == UnitSide.Player && _playerFleet.TryGetInfo(ship, out shipInfo))
                UpdateExperienceData(shipInfo);
        }

        private void UpdateExperienceData(IShipInfo ship)
        {
            if (ship == null)
                return;

            var total = _enemyFleet.GetExpForAllShips();
            if (total <= _totalExperience)
                return;

            long exp;
            if (!_playerExperienceData.TryGetValue(ship.ShipData, out exp))
                exp = 0;

            exp += (long)((total - _totalExperience) / (1f + ship.ShipData.Model.Layout.CellCount / 100f));
            _playerExperienceData[ship.ShipData] = exp;

            _totalExperience = total;
        }

        private long _totalExperience;
        private readonly Dictionary<IShip, long> _playerExperienceData = new Dictionary<IShip, long>();
        private readonly ShipCreatedSignal _shipCreatedSignal;
        private readonly ShipDestroyedSignal _shipDestroyedSignal;
    }
}

//namespace Combat
//{
//    public class CombatData
//    {
//        public enum CombatCompleteReason
//        {
//            ShipsDestroyed,
//            Cancelled,
//            Timeout,
//        }

//        public CombatData(IFleet firstFleet, IFleet secondFleet, CombatRules rules)
//        {
//            var enemyController = /*TODO: GameModel.Manager.Instance.DEBUG ? ControllerType.None :*/ ControllerType.Computer;
//            var random = new System.Random();
//            FirstFleet = new Fleet(ControllerType.Player, Model.OwnerType.Player, firstFleet.Ships, null, random.Next())
//            {
//                ZOrder = 1.1f,
//                AiLevel = firstFleet.AiLevel,
//                UseBonuses = !rules.DisableBonusses,
//                DisableExperience = true,
//            };

//            SecondFleet = new Fleet(enemyController, Model.OwnerType.Enemy, secondFleet.Ships, null, random.Next())
//            {
//                ZOrder = 1.0f,
//                AiLevel = secondFleet.AiLevel,
//                DisableExperience = rules.DisableExperience,
//            };

//            Rules = rules;
//            CompleteReason = CombatCompleteReason.Cancelled;
//        }

//        public CombatData(IEnumerable<IShip> firstFleet, IEnumerable<IShip> secondFleet, IRemoteClient remoteClient)
//        {
//            FirstFleet = new Fleet(ControllerType.PlayerDelayed, Model.OwnerType.Player, firstFleet, remoteClient, remoteClient.GameSeed) { ZOrder = 1.1f, DisableExperience = true };
//            SecondFleet = new Fleet(ControllerType.Remote, Model.OwnerType.Enemy, secondFleet, remoteClient, remoteClient.GameSeed) { ZOrder = 1.0f, DisableExperience = true };
//            RemoteClient = remoteClient;
//            Rules = Model.Factories.CombatRules.Multiplayer();
//            CompleteReason = CombatCompleteReason.Cancelled;
//        }

//        public Fleet FirstFleet { get; private set; }
//        public Fleet SecondFleet { get; private set; }
//        public CombatRules Rules { get; private set; }

//        public IEnumerable<IProduct> SpecialRewards { private get; set; }

//        public bool CanRetreat { get { return Rules.CanSelectShips && Rules.TimeoutBehaviour != TimeoutBehaviour.Decay && !IsMultiplayerGame; } }

//        public bool IsMultiplayerGame { get { return RemoteClient != null; } }
//        public IRemoteClient RemoteClient { get; private set; }

//        public CombatCompleteReason CompleteReason { get; set; }

//        public IEnumerable<IProduct> GetBattleRewards(LootGenerator lootGenerator, Galaxy.Star currentStar)
//        {
//            if (!RewardsAllowed)
//                yield break;

//            var money = 0;

//            if (SpecialRewards != null)
//            {
//                foreach (var item in SpecialRewards)
//                {
//                    if (item.Type is MoneyItem)
//                    {
//                        money += item.Quantity;
//                    }
//                    else
//                    {
//                        yield return item;
//                    }
//                }
//            }

//            if (Rules.RewardType == RewardType.SpecialOnly)
//            {
//                if (money > 0)
//                    yield return lootGenerator.GetMoney(money);

//                yield break;
//            }

//            var rewards = lootGenerator.GetCommonReward(
//                SecondFleet.Ships.Where(ship => SecondFleet.GetCondition(ship) <= 0),
//                currentStar.Level, currentStar.Region.Faction, currentStar.Id);

//            foreach (var item in rewards)
//            {
//                if (item.Type is MoneyItem)
//                {
//                    money += item.Quantity;
//                }
//                else
//                {
//                    yield return item;
//                }
//            }

//            if (money > 0)
//                yield return lootGenerator.GetMoney(money);
//        }

//        public bool RewardsAllowed
//        {
//            get
//            {
//                switch (Rules.RewardMode)
//                {
//                    case RewardMode.Always:
//                        return true;
//                    case RewardMode.Victory:
//                        return IsVictory;
//                    case RewardMode.None:
//                    default:
//                        return false;
//                }
//            }
//        }

//        public bool IsVictory { get { return !FirstFleet.IsEmpty && SecondFleet.IsEmpty; } }
//    }
//}

