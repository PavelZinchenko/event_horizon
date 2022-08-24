using System.Collections.Generic;
using System.Linq;
using Combat.Component.Unit.Classification;
using Combat.Domain;
using Economy.Products;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public class BattleNode : INode
    {
        public BattleNode(int id, ICombatModelBuilder builder, ILoot specialLoot)
        {
            _id = id;
            _specialLoot = specialLoot;
            _combatModelBuilder = builder;
        }

        public int Id { get { return _id; } }
        public NodeType Type { get { return NodeType.AttackFleet; } }

        public string GetRequirementsText(ILocalization localization)
        {
#if UNITY_EDITOR
            return GetType().Name + " - " + _id;
#else
            return Type.ToString();
#endif
        }

        public bool TryGetBeacons(ICollection<int> beacons) { return false; }

        public void Initialize() { _combatModel = null; }

        public INode VictoryNode { get; set; }
        public INode DefeatNode { get; set; }

        public bool TryProcessEvent(IQuestEventData eventData, out INode target)
        {
            target = this;
            if (eventData.Type != QuestEventType.CombatCompleted)
                return false;

            var data = (CombatEventData)eventData;
            if (data.CombatModel != _combatModel)
                return false;

            target = data.CombatModel.GetWinner() == UnitSide.Player ? VictoryNode : DefeatNode;
            return true;
        }

        public bool TryProceed(out INode target)
        {
            target = this;
            return false;
        }

        public bool ActionRequired { get { return _combatModel == null; } }
        public bool TryInvokeAction(IQuestActionProcessor processor)
        {
            var loot = _specialLoot != null ? _specialLoot.Items.Select(item => (IProduct)new Product(item.Type, item.Quantity)) : null;
            _combatModel = _combatModelBuilder.Build(loot);
            processor.StartCombat(_combatModel);
            return true;
        }

        private ICombatModel _combatModel;
        private readonly int _id;
        private readonly ILoot _specialLoot;
        private readonly ICombatModelBuilder _combatModelBuilder;
    }
}
