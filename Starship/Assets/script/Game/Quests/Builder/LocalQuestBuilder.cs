//using System.Collections.Generic;
//using System.Linq;
//using Economy.ItemType;
//using Economy.Products;
//using Galaxy;
//using GameDatabase;
//using GameDatabase.DataModel;
//using GameDatabase.Model;
//using GameModel.Quests.Nodes;
//using GameServices.Economy;
//using Zenject;

//namespace GameModel
//{
//    namespace Quests
//	{
//		public class LocalQuestBuilder
//		{
//            [Inject]
//		    public LocalQuestBuilder(
//                int starId,
//                int seed,
//                LootGenerator lootGenerator,
//                StarData starData,
//                ItemTypeFactory factory,
//		        CharacterFactory characterFactory,
//		        InventoryFactory inventoryFactory,
//                ConditionFactory conditionFactory,
//                NodeFactory nodeFactory,
//                IDatabase database)
//            {
//                _database = database;
//		        _lootGenerator = lootGenerator;
//		        _factory = factory;
//		        _starData = starData;
//		        _characterFactory = characterFactory;
//		        _inventoryFactory = inventoryFactory;
//                _nodeFactory = nodeFactory;
//                _conditionFactory = conditionFactory;
//                _starId = starId;
//                _seed = seed;
//            }

//			public IQuest Build()
//			{
//				var random = new System.Random(_seed);

//				INode completed = null;
//				INode node;

//				var value = random.Next(100);

//				if (value < 10)
//					node = DeadShipNode(completed, random);
//				else if (value < 25)
//					node = ShipWithoutFuelNode(completed, random);
//                else if (value < 40)
//                    node = MerchantShipsNode(completed);
//                else if (value < 45)
//				    node = AlienLifeformNode(completed);
//                else
//					node = AttackedByPiratesNode(completed);

//				return new LocalQuest(node, _starId);
//			}

//			private INode MerchantShipsNode(INode completed)
//			{
//				var character = _characterFactory.CreateMerchantCharacter(_starId, _starData.GetLevel(_starId));

//				var start = new CommonNode("$MERCHANT_Message", null);
//				var combat = _nodeFactory.CreateCombatNode(character);
//				var market = _nodeFactory.CreateMarketNode(character.Inventory, _inventoryFactory.CreatePlayerInventory());

//				start.AddTransition(new TextCondition("$MERCHANT_ACTION_Trade", Severity.Info), market);
//				start.AddTransition(new TextCondition("$MERCHANT_ACTION_Attack", Severity.Danger), combat);
//				start.AddTransition(new TextCondition("$MERCHANT_ACTION_Ignore", Severity.Info), completed);
//				combat.AddTransition(new CombatCompletedCondition(), completed);
//				market.DefaultTargetNode = completed;

//				return start;
//			}

//			private INode AttackedByPiratesNode(INode completed)
//			{
//				var character = _characterFactory.CreatePirateCharacter(_starId, _starData.GetLevel(_starId) + 10, new Product(_factory.CreateStarMapItem(_starId)));
//				var start = new FleetNode("$SOS_TRAP_Message", character);
//				var combat = _nodeFactory.CreateCombatNode(character);

//				start.AddTransition(new TextCondition("$ACTION_Continue", Severity.Danger), combat);
//				combat.AddTransition(new CombatCompletedCondition(), completed);

//				return start;
//			}

//		    private INode AlienLifeformNode(INode completed)
//		    {
//		        var level = _starData.GetLevel(_starId) + 10;
//                var fleet = Model.Factories.Fleet.SingleBoss(level, _database.GalaxySettings.AlienLifeformFaction, _seed, _database);
//		        var character = _characterFactory.CreateCharacter(fleet, Model.Factories.CombatRules.Flagship(level), _lootGenerator.GetSpaceWormLoot(level, _seed));
//		        var start = new FleetNode("$WORM_Message", character);
//		        var combat = _nodeFactory.CreateCombatNode(character);

//		        start.AddTransition(new TextCondition("$WORM_ACTION_Attack", Severity.Danger), combat);
//		        start.AddTransition(new TextCondition("$WORM_ACTION_Ignore", Severity.Info), completed);
//		        combat.AddTransition(new CombatCompletedCondition(), completed);

//		        return start;
//		    }

//			private INode ShipWithoutFuelNode(INode completed, System.Random random)
//			{
//				var character = _characterFactory.CreateCivilianCharacter(_starId, _starData.GetLevel(_starId)/2);
//				var fuel1 = new Product(_factory.CreateFuelItem(), 5);
//				var fuel2 = new Product(_factory.CreateFuelItem(), 10);
//				var rewardItems1 = new Reward((fuel1.Price*2).GetProduct(_factory), new Product(_factory.CreateStarMapItem(_starId)));
//				var rewardItems2 = new Reward((fuel1.Price*2).GetProduct(_factory), new Product(_factory.CreateStarMapItem(_starId)), GetRandomReward(random));

//				var start = new CommonNode("$SOS_OUTOFFUEL_Message", character);
//				var combat = _nodeFactory.CreateCombatNode(character);
//				var removeFuel1 = new RemoveItemsNode(fuel1.ToEnumerable<IProduct>());
//				var removeFuel2 = new RemoveItemsNode(fuel2.ToEnumerable<IProduct>());
//				var reward1 = new RewardNode("$SOS_OUTOFFUEL_Reward", null, rewardItems1);
//				var reward2 = new RewardNode("$SOS_OUTOFFUEL_Reward", null, rewardItems2);
//				var getReward1 = new GetRewardNode(rewardItems1);
//				var getReward2 = new GetRewardNode(rewardItems2);

//				start.AddTransition(_conditionFactory.CreateAskItemCondition("$SOS_OUTOFFUEL_ACTION_Give", fuel1, Severity.Info), removeFuel1);
//				start.AddTransition(_conditionFactory.CreateAskItemCondition("$SOS_OUTOFFUEL_ACTION_Give", fuel2, Severity.Info), removeFuel2);
//				start.AddTransition(new TextCondition("$SOS_OUTOFFUEL_ACTION_Attack", Severity.Danger), combat);
//				start.AddTransition(new TextCondition("$SOS_OUTOFFUEL_ACTION_Decline", Severity.Info), completed);
//				combat.AddTransition(new CombatCompletedCondition(), completed);
//				removeFuel1.TargetNode = reward1;
//				removeFuel2.TargetNode = reward2;
//				reward1.AddTransition(new TextCondition("$ACTION_Continue", Severity.Info), getReward1);
//				reward2.AddTransition(new TextCondition("$ACTION_Continue", Severity.Info), getReward2);
//				getReward1.TargetNode = completed;
//				getReward2.TargetNode = completed;

//				return start;
//			}

//			private INode DeadShipNode(INode completed, System.Random random)
//			{
//				var level = _starData.GetLevel(_starId);
//				var ship = _lootGenerator.GetRandomDamagedShip(level, random.Next());
//				var shipProduct = new Product(ship);
//				var money = shipProduct.Price.GetProduct(_factory);
//				var shipReward = new Reward(shipProduct);
//				var salvagedReward = new Reward(GetComponents(ship, random).ToArray());

//				var start = new RewardNode("$SOS_DEADSHIP_Message", null, shipReward);
//				var removeMoney = new RemoveItemsNode(money.ToEnumerable<IProduct>());
//				var repair = new GetRewardNode(shipReward);
//				var salvage = new RewardNode("$SOS_DEADSHIP_Salvaged", null, salvagedReward);
//				var getreward = new GetRewardNode(salvagedReward);

//				start.AddTransition(_conditionFactory.CreateAskItemCondition("$SOS_DEADSHIP_ACTION_Repair", money, Severity.Info), removeMoney);
//				start.AddTransition(new TextCondition("$SOS_DEADSHIP_ACTION_Salvage", Severity.Info), salvage);
//				removeMoney.TargetNode = repair;
//				repair.TargetNode = completed;
//				salvage.AddTransition(new TextCondition("$ACTION_Continue", Severity.Info), getreward);
//				getreward.TargetNode = completed;

//                return start;
//			}			

//			private INode NothingHappenedNode(INode completed)
//			{
//				var start = new CommonNode("$SOS_EMPTY_Message");
//				start.AddTransition(new TextCondition("$ACTION_Continue", Severity.Info), completed);
//				return start;
//			}			

//			private IProduct GetRandomReward(System.Random random)
//			{
//				return new Product(_lootGenerator.GetRandomComponent(_starData.GetLevel(_starId), Faction.Undefined, random.Next(), false));
//			}

//			private IEnumerable<IProduct> GetComponents(DamagedShipItem ship, System.Random random)
//			{
//                if (random.Next(2) == 0)
//					yield return new Product(_factory.CreateFuelItem(), random.Next(2, 5));
//				foreach (var item in ship.GetComponents())
//					yield return new Product(_factory.CreateComponentItem(item.Info));
//            }
            
//			private readonly int _starId;
//		    private readonly int _seed;

//		    private readonly IDatabase _database;
//            private readonly LootGenerator _lootGenerator;
//            private readonly ItemTypeFactory _factory;
//            private readonly StarData _starData;
//            private readonly CharacterFactory _characterFactory;
//            private readonly InventoryFactory _inventoryFactory;
//		    private readonly NodeFactory _nodeFactory;
//		    private readonly ConditionFactory _conditionFactory;
//		}
//    }
//}
