using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Skills;
using Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ViewModel.Skills
{
    public class SkillTreeNode : MonoBehaviour
    {
		public enum NodeState
		{
			Disabled,
			Enabled,
			EnabledAndConnected,
		}

        [Inject] private readonly ILocalization _localization;
		
        [SerializeField] SkillType _type;
        [SerializeField] int _multiplier = 1;
        [SerializeField] SkillTreeNode[] _linkedNodes;
        [SerializeField] UiLine _linkPrefab;
        [SerializeField] Image _icon;
        [SerializeField] float _sizeScale = 0.85f;
        [SerializeField] Color _lockedColor = new Color(0.3125f, 0.75f, 1f);
        [SerializeField] Color _unlockedColor = new Color(1f, 1f, 0.75f);
        [SerializeField] Color _lockedIconColor = new Color(0.5f, 1f, 1f);
        [SerializeField] Color _unlockedIconColor = new Color(1f, 1f, 0.75f);

        public float Size { get { return _sizeScale*GetComponent<RectTransform>().rect.width/2f; } }
        public IEnumerable<SkillTreeNode> LinkedNodes { get { return _linkedNodes; } }
		public string Name { get { return _type.GetName(_localization); } }
		public string Description { get { return _type.GetDescription(_localization, _multiplier); } }
        public SkillType Type { get { return _type; } }
        public int Multiplier { get { return _multiplier; } }

		public NodeState State
        {
            set
            {
                if (_state == value)
                    return;

                _state = value;
                UpdateState();
                UpdateLinks();
            }
        }

        public void ValidateLinks()
        {
            var valid = Array.IndexOf(_linkedNodes, null) < 0;
            if (valid)
                return;

            _linkedNodes = _linkedNodes.Where(item => item != null).ToArray();
        }

        public void AddLink(SkillTreeNode node)
        {
            _linkedNodes = _linkedNodes.AddIfNotExists(node);
        }

        public void RemoveLink(SkillTreeNode node)
        {
            _linkedNodes = _linkedNodes.RemoveAll(node);
        }

        public void ClearLinks()
        {
            _linkedNodes = new SkillTreeNode[] {};
        }

        private void Start()
        {
            CreateLines();
            UpdateState();
        }

        private void UpdateState()
        {
            var image = GetComponent<Image>();
            if (image)
				image.color = _state == NodeState.Disabled ? _lockedColor : _unlockedColor;

            if (_icon)
				_icon.color = _state == NodeState.Disabled ? _lockedIconColor : _unlockedIconColor;
        }

        private void CreateLines()
        {
            foreach (var item in _linkedNodes)
            {
                if (_links.ContainsKey(item))
                    continue;

                var link = CreateLink(item);
                AddLink(item, link);
                item.AddLink(this, link);
            }
        }

        private void UpdateLinks()
        {
            foreach (var link in _links)
                link.Value.color = IsLinkEnabled(link.Key) ? _unlockedColor : _lockedColor;
        }

        private void OnValidate()
        {
            if (_icon != null)
                _icon.sprite = CommonSpriteTable.SkillIcon(_type);
            if (_type != SkillType.Undefined)
                name = _type.ToString();
        }

        private void AddLink(SkillTreeNode node, UiLine link)
        {
            _links.Add(node, link);
        }

        private bool IsLinkEnabled(SkillTreeNode targetNode)
        {
			return _state == NodeState.EnabledAndConnected || targetNode._state == NodeState.EnabledAndConnected || _state == NodeState.Enabled && targetNode._state == NodeState.Enabled;
        }

        private UiLine CreateLink(SkillTreeNode node)
        {
            var link = Instantiate(_linkPrefab);
            link.transform.SetParent(transform.parent);
            link.transform.localScale = Vector3.one;

            link.color = IsLinkEnabled(node) ? _unlockedColor : _lockedColor;

            var begin = transform.localPosition;
            var end = node.transform.localPosition;
            var dir = (end - begin).normalized;

            link.SetPoints(begin + dir*Size, end - dir*node.Size);

            return link;
        }

		private NodeState _state = NodeState.Disabled;
        private readonly Dictionary<SkillTreeNode, UiLine> _links = new Dictionary<SkillTreeNode, UiLine>();
    }
}
