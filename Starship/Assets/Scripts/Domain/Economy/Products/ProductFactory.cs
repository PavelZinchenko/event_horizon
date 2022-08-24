using Economy.ItemType;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.Player;
using Session;
using Zenject;

namespace Economy.Products
{
    public class ProductFactory
    {
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly ItemTypeFactory _itemTypeFactory;
        [Inject] private readonly IDatabase _database;

        public IProduct CreatePlayerProduct(IItemType itemType, int quantity = 1)
        {
            return new PlayerProduct(_playerResources, itemType, quantity);
        }

        public IProduct CreateCargoHoldProduct(IItemType itemType, int quantity = 1)
        {
            return new PlayerProduct(_playerResources, itemType, quantity);
        }

        public IProduct CreateMarketProduct(IItemType itemType, int quantity = 1, float priceScale = 2f)
        {
            return new MarketProduct(_playerResources, itemType, quantity, priceScale);
        }

        public IProduct CreateArenaProduct(IItemType itemType, Price price)
        {
            return new ArenaProduct(_playerResources, itemType, price);
        }

        public IProduct CreateRenewableMarketProduct(IItemType itemType, int quantity, int marketId, long renewalTime, float priceScale = 2f)
        {
            return new RenewableMarketProduct(_playerResources, _session, itemType, quantity, marketId, renewalTime, priceScale);
        }

        public IProduct CreateSpecial(IItemType itemType, Price price)
        {
            return new SpecialProduct(_playerResources, itemType, price);
        }

        public IProduct CreateRandomComponentProduct(int marketId, int itemId, int itemLevel, Constructor.ComponentQuality maxQuality, Faction itemFaction, bool allowRare,
            long renewalTime, bool premium = false, float priceScale = 2f)
        {
            return new RandomComponentProduct(_database, _itemTypeFactory, _session, _playerResources, marketId, itemId, itemLevel, maxQuality, itemFaction, allowRare, renewalTime, premium, priceScale);
        }
    }
}
