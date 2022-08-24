using Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Services.Reources;
using Zenject;

namespace Gui.ComponentList
{
    public class GroupListItem : MonoBehaviour
    {
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;

        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _expandIcon;
        [SerializeField] private GameObject _collapseIcon;
        [SerializeField] private Text _nameText;
        [SerializeField] private Selectable _button;

        public void Initialize(IComponentTreeNode node, IComponentTreeNode activeNode)
        {
            Node = node;
            _icon.sprite = _resourceLocator.GetSprite(node.Icon);
            _icon.color = node.Color;
            _nameText.text = _localization.GetString(node.Name);

            var isParent = node.IsParent(activeNode);
            _expandIcon.gameObject.SetActive(!isParent && node != activeNode);
            _collapseIcon.gameObject.SetActive(isParent);
            _button.interactable = node != activeNode;
        }

        public IComponentTreeNode Node { get; private set; }
    }
}
