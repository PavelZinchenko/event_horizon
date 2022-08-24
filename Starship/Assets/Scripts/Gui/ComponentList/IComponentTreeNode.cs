using System.Collections.Generic;
using System.Linq;
using Constructor;
using GameDatabase.Model;
using Utils;

namespace Gui.ComponentList
{
    public interface IComponentTreeNode
    {
        IComponentTreeNode Parent { get; }
        IComponentQuantityProvider QuantityProvider { get; }
        IEnumerable<IComponentTreeNode> Nodes { get; }
        IEnumerable<ComponentInfo> Components { get; }
        int ItemCount { get; }

        string Name { get; }
        SpriteId Icon { get; }
        UnityEngine.Color Color { get; }

        void Add(ComponentInfo component);
        void Clear();
    }

    public static class ComponentTreeNodeExtensions
    {
        public static bool ShouldExpand(this IComponentTreeNode node)
        {
            if (node.Parent == null || node.Parent.QuantityProvider != node.QuantityProvider) return false;
            return node.ItemCount < MinElementsInGroup; 
            
        }

        public static bool ShouldNotExpand(this IComponentTreeNode node) { return !node.ShouldExpand(); }

        public static int GetItemCount(this IEnumerable<IComponentTreeNode> nodes)
        {
            var count = 0;
            foreach (var node in nodes)
                count += node.ShouldExpand() ? node.ItemCount : 1;

            return count;
        }

        public static IEnumerable<IComponentTreeNode> ChildrenNodes(this IEnumerable<IComponentTreeNode> nodes)
        {
            return nodes.Where(ShouldNotExpand).Concat(nodes.Where(ShouldExpand).SelectMany(node => node.Nodes));
        }

        public static IEnumerable<ComponentInfo> ChildrenComponents(this IEnumerable<IComponentTreeNode> nodes)
        {
            return nodes.Where(ShouldExpand).SelectMany(node => node.Components);
        }

        public static void Clear(this IEnumerable<IComponentTreeNode> nodes)
        {
            foreach (var node in nodes)
                node.Clear();
        }

        public static bool IsParent(this IComponentTreeNode self, IComponentTreeNode node)
        {
            if (node == null || node.Parent == null) return false;
            return node.Parent == self || self.IsParent(node.Parent);
        }

        public static IComponentTreeNode Root(this IComponentTreeNode node)
        {
            while (node.Parent != null)
                node = node.Parent;

            return node;
        }

        public static IComponentTreeNode NearestNode(this IComponentTreeNode node)
        {
            while (node.Parent != null && node.ShouldExpand())
                node = node.Parent;

            return node;
        }

        public static void Assign(this IComponentTreeNode node, IEnumerable<ComponentInfo> components)
        {
            node.Clear();

            foreach (var component in components)
                node.Add(component);
        }

        public static void Assign(this IComponentTreeNode node, IReadOnlyGameItemCollection<ComponentInfo> components)
        {
            node.Clear();

            foreach (var component in components.Items)
                node.Add(component.Key);
        }

        private const int MinElementsInGroup = 3;
    }
}
