using System;
using Galaxy;
using GameDatabase.DataModel;
using GameServices.Player;
using Services.InternetTime;
using Session;
using Utils;

namespace Domain.Quests
{
    public class QuestBuilder
    {
        public QuestBuilder(
            QuestModel model, 
            int starId, 
            int seed, 
            StarData starData,
            MotherShip motherShip,
            ISessionData session,
            Loot.Factory lootFactory, 
            FleetFactory fleetFactory,
            GameTime gameTime)
        {
            _model = model;
            _starId = starId;
            _seed = seed;
            _starData = starData;
            _motherShip = motherShip;
            _session = session;
            _lootCache = new LootCache(lootFactory);
            _enemyCache = new EnemyCache(fleetFactory);
            _requirementCache = new RequirementCache(_motherShip, _session, _lootCache, gameTime);
            _context = new QuestContext(_model, new Galaxy.Star(starId, starData), seed);
        }

        public Quest Build(int activeNodeId = 0)
        {
            var nodeBuilder = new NodeBuilder(_model, _context, _enemyCache, _requirementCache, _lootCache, _starData);
            var nodes = nodeBuilder.Build();

            if (nodes.Count == 0)
            {
                OptimizedDebug.LogException(new ArgumentException("QuestBuilder: quest has no nodes - " + _model.Id));
                return null;
            }

            INode activeNode;
            if (!nodes.TryGetValue(activeNodeId, out activeNode))
                activeNode = nodes.MinValue(item => (int)item.Key).Value;

            var quest = new Quest(_model, _starId, _seed);
            quest.Initialize(activeNode);
            return quest;
        }

        private readonly QuestModel _model;
        private readonly QuestContext _context;
        private readonly int _starId;
        private readonly int _seed;
        private readonly StarData _starData;
        private readonly MotherShip _motherShip;
        private readonly ISessionData _session;
        private readonly LootCache _lootCache;
        private readonly EnemyCache _enemyCache;
        private readonly RequirementCache _requirementCache;
    }
}
