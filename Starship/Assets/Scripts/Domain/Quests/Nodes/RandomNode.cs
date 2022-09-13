using System;
using System.Collections.Generic;
using System.Linq;
using GameDatabase.Enums;
using GameServices.Quests;
using Services.Localization;

namespace Domain.Quests
{
    public class RandomNode : INode
    {
        public RandomNode(int id, int seed, string description)
        {
            _id = id;
            var random = new Random(seed + id);
            _random = new Random(random.Next());
            _description = description;
        }

        public int Id { get { return _id; } }
        public NodeType Type { get { return NodeType.Random; } }
        public INode DefaultNode { get; set; }

        public string GetRequirementsText(ILocalization localization)
        {
            if (!string.IsNullOrEmpty(_description))
                return _description;

            return string.Join("\n", _transitions.Select(item =>
                item.Requirements.GetDescription(localization)).Where(item => !string.IsNullOrEmpty(item)).ToArray());
        }

        public bool TryGetBeacons(ICollection<int> beacons)
        {
            var result = false;

            foreach (var transition in _transitions)
            {
                if (transition.Requirements.BeaconPosition < 0) continue;
                beacons.Add(transition.Requirements.BeaconPosition);
                result = true;
            }

            return result;
        }

        public void Initialize() { }

        public void AddTransition(INodeRequirements requirements, INode node, float weight)
        {
            _transitions.Add(new Transition { Requirements = requirements, TargetNode = node, Weight = weight });
        }

        public bool TryProcessEvent(IQuestEventData eventData, out INode target)
        {
            return TryProceed(out target);
        }

        public bool TryProceed(out INode target)
        {
            List<Transition> allowedTransitions = null;
            var totalWeight = 0f;

            foreach (var transition in _transitions)
            {
                if (!transition.Requirements.IsMet) continue;

                if (allowedTransitions == null) allowedTransitions = new List<Transition>();
                allowedTransitions.Add(transition);
                totalWeight += transition.Weight;
            }

            if (allowedTransitions == null)
            {
                if (DefaultNode != null)
                {
                    target = DefaultNode;
                    return true;
                }

                target = this;
                return false;
            }


            if (totalWeight < 0.0001f)
            {
                target = allowedTransitions.RandomElement(_random).TargetNode;
                return true;
            }

            if (totalWeight < 1.0f) totalWeight = 1.0f;
            var value = _random.NextFloat()*totalWeight;

            foreach (var transition in allowedTransitions)
            {
                value -= transition.Weight;
                if (value > 0.0001f) continue;

                target = transition.TargetNode;
                return true;
            }
            
            if (DefaultNode != null)
            {
                target = DefaultNode;
                return true;
            }

            target = this;
            return false;
        }

        public bool ActionRequired { get { return false; } }
        public bool TryInvokeAction(IQuestActionProcessor processor) { return false; }

        private Random _random;
        private readonly string _description;
        private readonly int _id;
        private readonly List<Transition> _transitions = new List<Transition>();

        private struct Transition
        {
            public INodeRequirements Requirements;
            public INode TargetNode;
            public float Weight;
        }
    }
}
