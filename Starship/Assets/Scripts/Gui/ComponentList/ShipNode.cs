using System;
using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.Ships;
using GameDatabase.Model;
using Utils;

namespace Gui.ComponentList
{
    public class ShipNode : IComponentTreeNode
    {
        public ShipNode(IShip ship, IComponentTreeNode parent)
        {
            _ship = ship;
            _parent = parent;
            _quantityProvider = new ShipQuantityProvider(_ship);
        }

        public IShip Ship { get { return _ship; } }

        public IComponentTreeNode Parent { get { return _parent; } }
        public IComponentQuantityProvider QuantityProvider { get { return _quantityProvider; } }

        public bool IsVisible { get { return true; } }
        public bool IsExpanded { get; set; }

        public string Name { get { return "$GroupShip|" + _ship.Name; } }

        public SpriteId Icon { get { return _ship.Model.ModelImage; } }
        public UnityEngine.Color Color { get { return _ship.ColorScheme.Color; } }
        public void Add(ComponentInfo componentInfo) { throw new InvalidOperationException(); }
        public int ItemCount { get { return _quantityProvider.Components.Count; } }
        public IEnumerable<IComponentTreeNode> Nodes { get { return Enumerable.Empty<IComponentTreeNode>(); } }
        public IEnumerable<ComponentInfo> Components { get { return _quantityProvider.Components.Keys; } }
        public void Clear() { _quantityProvider.Reset(); }

        private readonly IShip _ship;
        private readonly IComponentTreeNode _parent;
        private readonly ShipQuantityProvider _quantityProvider;
    }

    public class ShipGroupNode : IComponentTreeNode
    {
        public ShipGroupNode(IComponentTreeNode parent)
        {
            _parent = parent;
        }

        public void Add(IShip ship)
        {
            _nodes.Add(new ShipNode(ship, this));
        }

        public IComponentTreeNode Parent { get { return _parent; } }
        public IComponentQuantityProvider QuantityProvider { get { return null; } }

        public bool IsVisible { get { return true; } }
        public bool IsExpanded { get; set; }

        public string Name { get { return "$GroupShips"; ; } }
        public SpriteId Icon { get { return new SpriteId("textures/icons/icon_fleet", SpriteId.Type.Default); } }
        public UnityEngine.Color Color { get { return CommonNode.DefaultColor; } }
        public void Add(ComponentInfo componentInfo) { throw new InvalidOperationException(); }
        public int ItemCount { get { return _nodes.Count; } }
        public IEnumerable<IComponentTreeNode> Nodes { get { return _nodes; } }
        public IEnumerable<ComponentInfo> Components { get { return Enumerable.Empty<ComponentInfo>(); } }

        public void Clear()
        {
            foreach (var node in _nodes)
                node.Clear();
        }

        private readonly List<IComponentTreeNode> _nodes = new List<IComponentTreeNode>();
        private readonly IComponentTreeNode _parent;
    }

    public class ShipQuantityProvider : IComponentQuantityProvider
    {
        public ShipQuantityProvider(IShip ship)
        {
            _ship = ship;
            Reset();
        }

        public int GetQuantity(ComponentInfo component)
        {
            return Components.GetQuantity(component);
        }

        public void Reset()
        {
            _components.Clear();
            _notInitialized = true;
        }

        public IGameItemCollection<ComponentInfo> Components
        {
            get
            {
                if (_notInitialized)
                {
                    foreach (var item in _ship.Components)
                        _components.Add(item.Info);

                    _notInitialized = false;
                }

                return _components;
            }
        }

        private bool _notInitialized;
        private readonly IShip _ship;
        private readonly IGameItemCollection<ComponentInfo> _components = new GameItemCollection<ComponentInfo>();
    }
}
