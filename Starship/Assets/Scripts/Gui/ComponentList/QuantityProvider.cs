using Constructor;
using Constructor.Ships;
using GameDatabase.DataModel;
using Utils;

namespace Gui.ComponentList
{
    public interface IComponentQuantityProvider
    {
        int GetQuantity(ComponentInfo component);
    }

    public class ComponentQuantityProvider : IComponentQuantityProvider
    {
        public ComponentQuantityProvider(IReadOnlyGameItemCollection<ComponentInfo> components)
        {
            _components = components;
        }

        public int GetQuantity(ComponentInfo component)
        {
            return _components.GetQuantity(component);
        }

        private readonly IReadOnlyGameItemCollection<ComponentInfo> _components;
    }

    public class BlueprintQuantityProvider : IComponentQuantityProvider
    {
        public BlueprintQuantityProvider(IReadOnlyGameItemCollection<Component> blueprints)
        {
            _blueprints = blueprints;
        }

        public int GetQuantity(ComponentInfo component)
        {
            return _blueprints.GetQuantity(component.Data);
        }

        private readonly IReadOnlyGameItemCollection<Component> _blueprints;
    }
}
