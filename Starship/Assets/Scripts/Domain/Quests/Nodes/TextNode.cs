using System.Collections.Generic;
using System.Linq;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public class TextNode : INode
    {
        public TextNode(int id, string message, string characterName, SpriteId characterAvatar, Model.Military.IFleet enemy, ILoot loot, RequiredViewMode requiredView)
        {
            _id = id;
            _message = message;
            _characterName = characterName;
            _characterAvatar = characterAvatar;
            _enemy = enemy;
            _loot = loot;
            _requiredView = requiredView;
        }

        public int Id { get { return _id; } }
        public NodeType Type { get { return NodeType.ShowDialog; } }

        public string GetRequirementsText(ILocalization localization)
        {
//#if UNITY_EDITOR
//            text.AppendLine(GetType().Name + " - " + _id);
//#endif

            return string.Join("\n", _transitionList.Select(item => 
                item.Requirements.GetDescription(localization)).Where(item => !string.IsNullOrEmpty(item)).ToArray());
        }

        public bool TryGetBeacons(ICollection<int> beacons)
        {
            var result = false;

            foreach (var transition in _transitionList)
            {
                if (transition.Requirements.BeaconPosition < 0) continue;
                beacons.Add(transition.Requirements.BeaconPosition);
                result = true;
            }

            return result;
        }

        public void Initialize()
        {
            ActionRequired = _transitionList.Exists(item => item.Requirements.IsMet);
        }

        public void AddAction(string buttonText, Severity severity, INodeRequirements requirements, INode node)
        {
            _transitionList.Add(new Transition { ButtonText = buttonText, Requirements = requirements, TargetNode = node, Severity = severity });
        }

        public bool TryProcessEvent(IQuestEventData eventData, out INode target)
        {
            if (eventData.Type == QuestEventType.ActionButtonPressed)
            {
                var data = (ButtonPressedEventData)eventData;
                if (data.Context != this)
                {
                    target = this;
                    return false;
                }

                var transition = _transitionList[data.Id];
                if (!transition.Requirements.IsMet)
                {
                    UnityEngine.Debug.LogError("TextNode: Requirements are not met - " + transition.Requirements.GetType().Name);
                    target = this;
                    return false;
                }

                target = transition.TargetNode;
                return true;
            }

            ActionRequired = _transitionList.Exists(item => item.Requirements.IsMet);
            target = this;
            return false;
        }

        public bool TryProceed(out INode target)
        {
            target = this;
            return false;
        }

        public bool ActionRequired { get; private set; }

        public bool TryInvokeAction(IQuestActionProcessor processor)
        {
            var actions = new List<UserAction>();
            ActionRequired = false;

            for (var i = 0; i < _transitionList.Count; ++i)
            {
                var transition = _transitionList[i];
                if (!transition.Requirements.IsMet) continue;

                actions.Add(new UserAction(
                    new ButtonPressedEventData(this, i),
                    transition.ButtonText,
                    transition.Requirements,
                    transition.Severity));
            }

            if (actions.Count == 0) return false;

            var interaction = new UserInteraction
            {
                Actions = actions,
                Message = _message,
                CharacterName = _characterName,
                CharacterAvatar = _characterAvatar,
                RequiredView = _requiredView,
                Enemies = _enemy ?? Model.Factories.Fleet.Empty,
                Loot = _loot ?? EmptyLoot.Instance,
            };

            processor.ShowUiDialog(interaction);
            return true;
        }

        private readonly int _id;
        private readonly RequiredViewMode _requiredView;
        private readonly string _message;
        private readonly string _characterName;
        private readonly Model.Military.IFleet _enemy;
        private readonly ILoot _loot;
        private readonly SpriteId _characterAvatar;
        private readonly List<Transition> _transitionList = new List<Transition>();

        private struct Transition
        {
            public INodeRequirements Requirements;
            public INode TargetNode;
            public string ButtonText;
            public Severity Severity;
        }
    }
}
