using GameDatabase.DataModel;
using GameModel;
using GameServices.Player;
using Services.InternetTime;
using Session;
using Zenject;

namespace Domain.Quests
{
    public class RequirementsFactory
    {
        [Inject]
        public RequirementsFactory(MotherShip motherShip, Loot.Factory lootFactory, ISessionData session, RegionMap regionMap, GameTime gameTime)
        {
            _session = session;
            _regionMap = regionMap;
            _motherShip = motherShip;
            _gameTime = gameTime;
            _lootCache = new LootCache(lootFactory);
        }

        // TODO: move to another place
        public QuestGiver CreateQuestGiver(GameDatabase.DataModel.QuestOrigin data)
        {
            return new QuestGiver(data, _regionMap, _session);
        }

        public IQuestRequirements CreateForQuest(Requirement requirement, int seed)
        {
            if (requirement == null)
                return EmptyRequirements.Instance;

            var builder = new RequirementsBuilder(new QuestContext(seed), _lootCache, _motherShip, _session, _gameTime);
            return requirement.Create(builder);
        }

        public INodeRequirements CreateForNode(Requirement requirement, QuestContext context)
        {
            if (requirement == null)
                return EmptyRequirements.Instance;

            var builder = new RequirementsBuilder(context, _lootCache, _motherShip, _session, _gameTime);
            return requirement.Create(builder);
        }

        private readonly ILootCache _lootCache;
        private readonly ISessionData _session;
        private readonly RegionMap _regionMap;
        private readonly MotherShip _motherShip;
        private readonly GameTime _gameTime;

        private class LootCache : ILootCache
        {
            public LootCache(Loot.Factory lootFactory)
            {
                _lootFactory = lootFactory;
            }

            public ILoot Get(LootModel model, QuestContext context)
            {
                return _lootFactory.Create(model, context);
            }

            private readonly Loot.Factory _lootFactory;
        }
    }
}
