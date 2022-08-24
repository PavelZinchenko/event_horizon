using System.Collections.Generic;
using System.Linq;
using Galaxy;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace Domain.Quests
{
    public class NodeBuilder : INodeFactory<INode>
    {
        public NodeBuilder(QuestModel data, QuestContext context, IEnemyCache enemyCache, IRequirementCache requirementCache, ILootCache lootCache, StarData starData)
        {
            _context = context;
            _starData = starData;
            _enemyCache = enemyCache;
            _requirementCache = requirementCache;
            _lootCache = lootCache;

            _nodes = new Dictionary<NodeId, INode>();
            _nodesData = data.Nodes.ToDictionary(node => new NodeId(node.Id));
        }

        public Dictionary<NodeId, INode> Build()
        {
            foreach (var id in _nodesData.Keys)
                CreateNode(id);

            return _nodes;
        }

        private INode CreateNode(NodeId id)
        {
            if (_nodes.TryGetValue(id, out var node))
                return node;

            if (!_nodesData.TryGetValue(id, out var data))
                return null;

            return data.Create(this);
        }

        private INode TerminalNode(NodeId id, NodeType type)
        {
            var node = new TerminalNode(id, type);
            _nodes.Add(node.Id, node);
            return node;
        }

        public INode Create(Node_Undefined content)
        {
            return TerminalNode(content.Id, NodeType.Undefined);
        }

        public INode Create(Node_ComingSoon content)
        {
            return TerminalNode(content.Id, NodeType.ComingSoon);
        }

        public INode Create(Node_ShowDialog content)
        {
            var characterName = content.Character?.Name;
            var characterAvatar = content.Character?.AvatarIcon ?? SpriteId.Empty;
            var enemy = _enemyCache.Get(content.Enemy, _context);
            var items = _lootCache.Get(content.Loot, _context);

            var node = new TextNode(content.Id, content.Message, characterName, characterAvatar, enemy?.EnemyFleet, items, content.RequiredView);
            _nodes.Add(node.Id, node);

            foreach (var action in content.Actions)
            {
                var targetNode = CreateNode(action.TargetNode);
                var requirements = _requirementCache.Get(action.Requirement, _context);
                node.AddAction(action.ButtonText, Severity.Info, requirements, targetNode);
            }

            return node;
        }

        public INode Create(Node_Switch content)
        {
            var node = new SwitchNode(content.Id, content.Message);
            _nodes.Add(node.Id, node);

            node.DefaultNode = CreateNode(content.DefaultTransition);

            foreach (var item in content.Transitions)
            {
                var targetNode = CreateNode(item.TargetNode);
                var requirements = _requirementCache.Get(item.Requirement, _context);
                node.AddTransition(requirements, targetNode);
            }

            return node;
        }

        public INode Create(Node_Condition content)
        {
            var node = new ConditionNode(content.Id, content.Message);
            _nodes.Add(node.Id, node);

            var transition = content.Transitions.First();

            node.TargetNode = CreateNode(transition.TargetNode);
            node.Requirements = _requirementCache.Get(transition.Requirement, _context);

            return node;
        }

        public INode Create(Node_Random content)
        {
            var node = new RandomNode(content.Id, _context.Seed, content.Message);
            _nodes.Add(node.Id, node);

            node.DefaultNode = CreateNode(content.DefaultTransition);

            foreach (var item in content.Transitions)
            {
                var targetNode = CreateNode(item.TargetNode);
                var requirements = _requirementCache.Get(item.Requirement, _context);
                node.AddTransition(requirements, targetNode, item.Weight);
            }

            return node;
        }

        public INode Create(Node_Retreat content)
        {
            var node = new RetreatNode(content.Id);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_DestroyOccupants content)
        {
            var node = new SuppressOccupantNode(content.Id, _context.StarId, true);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_SuppressOccupants content)
        {
            var node = new SuppressOccupantNode(content.Id, _context.StarId, false);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_ReceiveItem content)
        {
            var loot = _lootCache.Get(content.Loot, _context);
            var node = new LootNode(content.Id, loot);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_RemoveItem content)
        {
            var loot = _lootCache.Get(content.Loot, _context);
            var node = new RemoveItemNode(content.Id, loot);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_Trade content)
        {
            var loot = _lootCache.Get(content.Loot, _context);
            var node = new TradingNode(content.Id, loot);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_CompleteQuest content)
        {
            return TerminalNode(content.Id, NodeType.CompleteQuest);
        }

        public INode Create(Node_FailQuest content)
        {
            return TerminalNode(content.Id, NodeType.FailQuest);
        }

        public INode Create(Node_CancelQuest content)
        {
            return TerminalNode(content.Id, NodeType.CancelQuest);
        }

        public INode Create(Node_StartQuest content)
        {
            var node = new StartQuestNode(content.Id, content.Quest);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_ChangeFactionRelations content)
        {
            var starId = _starData.GetRegion(_context.StarId).HomeStar;
            var node = new FactionRelationsNode(content.Id, starId, content.Value, true);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_CaptureStarBase content)
        {
            var starId = _starData.GetRegion(_context.StarId).HomeStar;
            var node = new CaptureStarBaseNode(content.Id, starId, true);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_LiberateStarBase content)
        {
            var starId = _starData.GetRegion(_context.StarId).HomeStar;
            var node = new CaptureStarBaseNode(content.Id, starId, false);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_ChangeFaction content)
        {
            var starId = _starData.GetRegion(_context.StarId).HomeStar;
            var node = new ChangeFactionNode(content.Id, starId, content.Faction);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_SetFactionRelations content)
        {
            var starId = _starData.GetRegion(_context.StarId).HomeStar;
            var node = new FactionRelationsNode(content.Id, starId, content.Value, false);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_ChangeCharacterRelations content)
        {
            var node = new CharacterRelationsNode(content.Id, content.Character.Id.Value, content.Value, true);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_SetCharacterRelations content)
        {
            var node = new CharacterRelationsNode(content.Id, content.Character.Id.Value, content.Value, false);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_OpenShipyard content)
        {
            var faction = content.Faction != Faction.Undefined ? content.Faction : _starData.GetRegion(_context.StarId).Faction;
            var level = content.Level > 0 ? content.Level : _starData.GetLevel(_context.StarId);

            var node = new ShipyardNode(content.Id, faction, level);
            _nodes.Add(node.Id, node);
            node.TargetNode = CreateNode(content.Transition);
            return node;
        }

        public INode Create(Node_AttackFleet content)
        {
            var node = new BattleNode(content.Id, _enemyCache.Get(content.Enemy, _context), _lootCache.Get(content.Loot, _context));
            _nodes.Add(node.Id, node);
            node.VictoryNode = CreateNode(content.VictoryTransition);
            node.DefeatNode = CreateNode(content.FailureTransition);
            return node;
        }

        public INode Create(Node_AttackOccupants content)
        {
            var node = new BattleNode(content.Id, _starData.GetOccupant(_context.StarId).CreateCombatModelBuilder(), null);
            _nodes.Add(node.Id, node);
            node.VictoryNode = CreateNode(content.VictoryTransition);
            node.DefeatNode = CreateNode(content.FailureTransition);
            return node;
        }

        private readonly QuestContext _context;
        private readonly Dictionary<NodeId, INode> _nodes;
        private readonly Dictionary<NodeId, Node> _nodesData;
        private readonly StarData _starData;
        private readonly IEnemyCache _enemyCache;
        private readonly ILootCache _lootCache;
        private readonly IRequirementCache _requirementCache;
    }
}
