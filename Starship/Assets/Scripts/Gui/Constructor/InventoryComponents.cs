using System.Collections.Generic;
using System.Linq;
using Constructor;
using GameDatabase.Enums;
using GameServices.Player;
using Utils;

namespace Gui.Constructor
{
    public class InventoryComponents
    {
        public IReadOnlyGameItemCollection<ComponentInfo> Items => _components;

        public void Add(ComponentInfo item)
        {
            _components.Add(item);
        }

        public void Remove(ComponentInfo item)
        {
            _components.Remove(item);
        }

        public bool Contains(ComponentInfo item)
        {
            return _components.GetQuantity(item) > 0;
        }

        public void LoadFromInventory(PlayerInventory inventory)
        {
            _components.Assign(inventory.Components.Items);
        }

        public void SaveToInventory(PlayerInventory inventory)
        {
            inventory.Components.Assign(_components.Items.Where(item => item.Value > 0).Select(item => new KeyValuePair<ComponentInfo, int>(item.Key, item.Value)));
        }

        public void LoadFromDatabase(GameDatabase.IDatabase database)
        {
            _components.Clear();
            foreach (var item in database.ComponentList)
            {
                var common = new ComponentInfo(item);
                _components.Add(common, 999);
                foreach (var mod in item.PossibleModifications)
                {
                    var component = new ComponentInfo(item, mod.Type, ModificationQuality.P3);
                    _components.Add(component, 999);
                }
            }
        }

        public readonly GameItemCollection<ComponentInfo> _components = new GameItemCollection<ComponentInfo>();
    }
}
