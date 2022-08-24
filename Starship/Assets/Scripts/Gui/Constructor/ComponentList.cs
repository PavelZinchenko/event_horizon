using UnityEngine;
using Constructor;
using Gui.ComponentList;
using Utils;

namespace Gui.Constructor
{
    public class ComponentList : MonoBehaviour
    {
        //[SerializeField] private ComponentInfoViewModel _cComponentInfo;
        [SerializeField] private ComponentContentFiller _contentFiller;
        [SerializeField] private ListScrollRect _componentList;
        [SerializeField] private GameObject _noItemsText;

        public void Initialize(IReadOnlyGameItemCollection<ComponentInfo> components)
        {
            _componentQuantityProvider = new ComponentQuantityProvider(components);
            _rootNode = new RootNode(_componentQuantityProvider);
            _rootNode.Assign(components);
            _selectedNode = _rootNode;
            _components = components;
            _noItemsText.SetActive(components.Count == 0);
            RefreshList();
        }

        public void ShowAll()
        {
            _selectedNode = _rootNode;
            RefreshList();
        }

        public void ShowWeapon()
        {
            _selectedNode = _rootNode.Weapon;
            RefreshList();
        }

        public void ShowArmor()
        {
            _selectedNode = _rootNode.Armor;
            RefreshList();
        }

        public void ShowEngine()
        {
            _selectedNode = _rootNode.Engine;
            RefreshList();
        }

        public void ShowEnergy()
        {
            _selectedNode = _rootNode.Energy;
            RefreshList();
        }

        public void ShowDrone()
        {
            _selectedNode = _rootNode.Drone;
            RefreshList();
        }

        public void ShowSpecial()
        {
            _selectedNode = _rootNode.Special;
            RefreshList();
        }

        public void OnGroupSelected(GroupListItem item)
        {
            _selectedNode = item.Node;
            RefreshList();
        }

        public void RefreshList()
        {
            _rootNode.Assign(_components);
            _contentFiller.InitializeItems(_selectedNode);
            _componentList.RefreshContent();
            _noItemsText.SetActive(_components.Count == 0);
        }

        private RootNode _rootNode;
        private IComponentTreeNode _selectedNode;
        private ComponentQuantityProvider _componentQuantityProvider;
        private IReadOnlyGameItemCollection<ComponentInfo> _components;
    }
}
