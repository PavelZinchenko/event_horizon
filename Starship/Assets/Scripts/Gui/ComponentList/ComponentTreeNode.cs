using System.Collections.Generic;
using System.Linq;
using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace Gui.ComponentList
{
    public class RootNode : IComponentTreeNode
    {
        public RootNode(IComponentQuantityProvider quantityProvider)
        {
            _quantityProvider = quantityProvider;
            _weaponNode = new WeaponNode(this);

            _armorNode = CreateNode("$GroupArmor", new SpriteId("textures/icons/icon_shield", SpriteId.Type.Default));
            _energyNode = CreateNode("$GroupEnergy", new SpriteId("textures/icons/icon_battery", SpriteId.Type.Default));
            _droneNode = CreateNode("$GroupDrones", new SpriteId("textures/icons/icon_drone", SpriteId.Type.Default));
            _engineNode = CreateNode("$GroupEngines", new SpriteId("textures/icons/icon_engine", SpriteId.Type.Default));
            _specialNode = CreateNode("$GroupSpecial", new SpriteId("textures/icons/icon_gear", SpriteId.Type.Default));
        }

        public void AddNode(IComponentTreeNode node)
        {
            _extraNodes.Add(node);
        }

        public IComponentTreeNode Parent { get { return null; } }
        public IComponentQuantityProvider QuantityProvider { get { return _quantityProvider; } }

        public IComponentTreeNode Weapon { get { return _weaponNode; } }
        public IComponentTreeNode Armor { get { return _armorNode; } }
        public IComponentTreeNode Drone { get { return _droneNode; } }
        public IComponentTreeNode Engine { get { return _engineNode; } }
        public IComponentTreeNode Energy { get { return _energyNode; } }
        public IComponentTreeNode Special { get { return _specialNode; } }

        public string Name { get { return "$GroupAll"; } }
        public SpriteId Icon { get { return new SpriteId("textures/icons/icon_gear", SpriteId.Type.Default); } }
        public UnityEngine.Color Color { get { return CommonNode.DefaultColor; } }

        public void Add(ComponentInfo componentInfo)
        {
            if (componentInfo.Data.Weapon != null)
            {
                _weaponNode.Add(componentInfo);
                return;
            }

            switch (componentInfo.Data.DisplayCategory)
            {
                case ComponentCategory.Defense:
                    _armorNode.Add(componentInfo);
                    break;
                case ComponentCategory.Energy:
                    _energyNode.Add(componentInfo);
                    break;
                case ComponentCategory.Engine:
                    _engineNode.Add(componentInfo);
                    break;
                case ComponentCategory.Drones:
                    _droneNode.Add(componentInfo);
                    break;
                default:
                    _specialNode.Add(componentInfo);
                    break;
            }

            _count = -1;
        }

        public int ItemCount
        {
            get
            {
                if (_count < 0)
                    _count = Children.GetItemCount();

                return _count;
            }
        }

        public IEnumerable<IComponentTreeNode> Nodes
        {
            get
            {
                return Children.ChildrenNodes();
            }
        }

        public IEnumerable<ComponentInfo> Components { get { return Children.ChildrenComponents(); } }

        public void Clear() { Children.Clear(); }

        private IEnumerable<IComponentTreeNode> Children
        {
            get
            {
                foreach (var node in _extraNodes)
                    yield return node;

                yield return _weaponNode;
                yield return _armorNode;
                yield return _energyNode;
                yield return _droneNode;
                yield return _engineNode;
                yield return _specialNode;
            }
        }

        private IComponentTreeNode CreateNode(string name, SpriteId icon)
        {
            return new CommonNode(name, icon, this);
        }

        private int _count = -1;
        private readonly IComponentTreeNode _weaponNode;
        private readonly IComponentTreeNode _armorNode;
        private readonly IComponentTreeNode _energyNode;
        private readonly IComponentTreeNode _droneNode;
        private readonly IComponentTreeNode _engineNode;
        private readonly IComponentTreeNode _specialNode;
        private readonly IComponentQuantityProvider _quantityProvider;
        private readonly List<IComponentTreeNode> _extraNodes = new List<IComponentTreeNode>();
    }

    public class WeaponNode : IComponentTreeNode
    {
        public WeaponNode(IComponentTreeNode parent)
        {
            _parent = parent;
            _projectileNode = CreateNode("$GroupWeaponC", new SpriteId("textures/weapongroup/icon_weapon_c", SpriteId.Type.Default));
            _beamNode = CreateNode("$GroupWeaponL", new SpriteId("textures/weapongroup/icon_weapon_l", SpriteId.Type.Default));
            _missileNode = CreateNode("$GroupWeaponM", new SpriteId("textures/weapongroup/icon_weapon_m", SpriteId.Type.Default));
            _torpedoNode = CreateNode("$GroupWeaponT", new SpriteId("textures/weapongroup/icon_weapon_t", SpriteId.Type.Default));
            _specialNode = CreateNode("$GroupWeaponS", new SpriteId("textures/weapongroup/icon_weapon_s", SpriteId.Type.Default));
            _universalNode = CreateNode("$GroupWeaponAny", new SpriteId("textures/weapongroup/icon_weapon_x", SpriteId.Type.Default));
        }

        public IComponentTreeNode Parent { get { return _parent; } }
        public IComponentQuantityProvider QuantityProvider { get { return _parent.QuantityProvider; } }

        public string Name { get { return "$GroupWeapon"; } }
        public SpriteId Icon { get { return new SpriteId("textures/icons/icon_weapon", SpriteId.Type.Default); } }
        public UnityEngine.Color Color { get { return CommonNode.DefaultColor; } }

        public void Add(ComponentInfo componentInfo)
        {
            var weapon = componentInfo.Data.Weapon;
            if (weapon == null)
            {
                UnityEngine.Debug.LogError("WeaponNode: component is not weapon - " + componentInfo.Data.Id);
                return;
            }

            switch (componentInfo.Data.WeaponSlotType)
            {
                case WeaponSlotType.Cannon:
                    _projectileNode.Add(componentInfo);
                    break;
                case WeaponSlotType.Laser:
                    _beamNode.Add(componentInfo);
                    break;
                case WeaponSlotType.Missile:
                    _missileNode.Add(componentInfo);
                    break;
                case WeaponSlotType.Torpedo:
                    _torpedoNode.Add(componentInfo);
                    break;
                case WeaponSlotType.Special:
                    _specialNode.Add(componentInfo);
                    break;
                default:
                    _universalNode.Add(componentInfo);
                    break;
            }
        }

        public int ItemCount
        {
            get
            {
                if (_count < 0)
                    _count = Children.GetItemCount();

                return _count;
            }
        }

        public IEnumerable<IComponentTreeNode> Nodes { get { return Children.ChildrenNodes(); } }
        public IEnumerable<ComponentInfo> Components { get { return Children.ChildrenComponents(); } }
        public void Clear() { Children.Clear(); }

        private IEnumerable<IComponentTreeNode> Children
        {
            get
            {
                yield return _projectileNode;
                yield return _beamNode;
                yield return _missileNode;
                yield return _torpedoNode;
                yield return _specialNode;
                yield return _universalNode;
            }
        }

        private IComponentTreeNode CreateNode(string name, SpriteId icon)
        {
            return new CommonNode(name, icon, this);
        }

        private int _count = -1;
        private readonly IComponentTreeNode _parent;
        private readonly IComponentTreeNode _projectileNode;
        private readonly IComponentTreeNode _beamNode;
        private readonly IComponentTreeNode _missileNode;
        private readonly IComponentTreeNode _torpedoNode;
        private readonly IComponentTreeNode _specialNode;
        private readonly IComponentTreeNode _universalNode;
    }

    public class ComponentNode : IComponentTreeNode
    {
        public ComponentNode(Component component, IComponentTreeNode parent)
        {
            _component = component;
            _parent = parent;
        }

        public IComponentTreeNode Parent { get { return _parent; } }
        public IComponentQuantityProvider QuantityProvider { get { return _parent.QuantityProvider; } }

        public string Name { get { return _component.Name; } }
        public SpriteId Icon { get { return _component.Icon; } }
        public UnityEngine.Color Color { get { return _component.Color; } }

        public void Add(ComponentInfo componentInfo)
        {
            if (componentInfo.Data.Id != _component.Id)
            {
                UnityEngine.Debug.LogError("ComponentNode: wrong component id - " + componentInfo.Data.Id);
                return;
            }

            _components.Add(componentInfo);
        }

        public int ItemCount { get { return _components.Count; } }
        public IEnumerable<IComponentTreeNode> Nodes { get { return Enumerable.Empty<IComponentTreeNode>(); } }
        public IEnumerable<ComponentInfo> Components { get { return _components; } }

        public void Clear()
        {
            _components.Clear();
        }

        private readonly Component _component;
        private readonly IComponentTreeNode _parent;
        private readonly HashSet<ComponentInfo> _components = new HashSet<ComponentInfo>();
    }

    public class CommonNode : IComponentTreeNode
    {
        public CommonNode(string name, SpriteId icon, IComponentTreeNode parent)
        {
            _parent = parent;
            _name = name;
            _icon = icon;
        }

        public IComponentTreeNode Parent { get { return _parent; } }
        public IComponentQuantityProvider QuantityProvider { get { return _parent.QuantityProvider; } }

        public string Name { get { return _name; } }
        public SpriteId Icon { get { return _icon; } }
        public UnityEngine.Color Color { get { return DefaultColor; } }

        public void Add(ComponentInfo componentInfo)
        {
            var component = componentInfo.Data;
            IComponentTreeNode node;
            if (!_components.TryGetValue(component.Id.Value, out node))
            {
                node = new ComponentNode(component, this);
                _components.Add(component.Id.Value, node);
            }

            node.Add(componentInfo);
            _count = -1;
        }

        public int ItemCount
        {
            get
            {
                if (_count < 0)
                    _count = _components.Values.GetItemCount();

                return _count;
            }
        }

        public IEnumerable<IComponentTreeNode> Nodes { get { return _components.Values.Where(ComponentTreeNodeExtensions.ShouldNotExpand); } }
        public IEnumerable<ComponentInfo> Components { get { return _components.Values.ChildrenComponents(); } }

        public void Clear()
        {
            _components.Values.Clear();
        }

        private int _count;
        private readonly string _name;
        private readonly SpriteId _icon;
        private readonly IComponentTreeNode _parent;
        private readonly Dictionary<int, IComponentTreeNode> _components = new Dictionary<int, IComponentTreeNode>();

        public static readonly UnityEngine.Color DefaultColor = new UnityEngine.Color(0.75f,0.75f,0.5f,1.0f);
    }

    public class ComponentListNode : IComponentTreeNode
    {
        public ComponentListNode(string name, SpriteId icon, IComponentTreeNode parent)
        {
            _parent = parent;
            _name = name;
            _icon = icon;
        }

        public IComponentTreeNode Parent { get { return _parent; } }
        public IComponentQuantityProvider QuantityProvider { get { return _parent.QuantityProvider; } }

        public string Name { get { return _name; } }
        public SpriteId Icon { get { return _icon; } }
        public UnityEngine.Color Color { get { return CommonNode.DefaultColor; } }

        public void Add(ComponentInfo componentInfo)
        {
            _components.Add(componentInfo);
        }

        public int ItemCount { get { return _components.Count; } }

        public IEnumerable<IComponentTreeNode> Nodes { get { return Enumerable.Empty<IComponentTreeNode>(); } }
        public IEnumerable<ComponentInfo> Components { get { return _components; } }

        public void Clear()
        {
            _components.Clear();
        }

        private readonly string _name;
        private readonly SpriteId _icon;
        private readonly IComponentTreeNode _parent;
        private readonly HashSet<ComponentInfo> _components = new HashSet<ComponentInfo>();
    }
}
