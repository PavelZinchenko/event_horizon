using Constructor;
using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Economy.ItemType
{
    public class ComponentItem : IItemType
    {
        [Inject]
        public ComponentItem(PlayerInventory inventory, ILocalization localization, ComponentInfo info, bool premium = false)
        {
            _localization = localization;
            _inventory = inventory;
            Component = info;
            _premium = premium;
        }

        public string Id { get { return "c" + Component.SerializeToInt64(); } }
        public string Name { get { return Component.GetName(_localization); } }
        public string Description { get { return Component.CreateModification().GetDescription(_localization); } }
        public Sprite GetIcon(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(Component.Data.Icon); }
        public Color Color { get { return Component.Data.Color; } }
        public Price Price { get { return _premium ? Component.PremiumPrice : Component.Price; } }
        public ItemQuality Quality { get { return Component.ItemQuality; } }

        public void Consume(int amount)
        {
            _inventory.Components.Add(Component, amount);
        }

        public void Withdraw(int amount)
        {
            _inventory.Components.Remove(Component, amount);
        }

        public int MaxItemsToConsume { get { return int.MaxValue; } }
        public int MaxItemsToWithdraw { get { return _inventory.Components.GetQuantity(Component); } }

        public ComponentInfo Component { get; private set; }

        private readonly bool _premium;
        private readonly PlayerInventory _inventory;
        private readonly ILocalization _localization;
    }
}
