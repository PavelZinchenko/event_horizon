using Economy.ItemType;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Player;
using Session;

namespace Economy.Products
{
    public class Product : IProduct
    {
        public Product(IItemType type, int quantity = 1)
        {
            Type = type;
            Quantity = quantity;
        }

        public IItemType Type { get; private set; }
        public int Quantity { get { return _quantity; } private set { _quantity = value; } }
        public Price Price { get { return Type.Price; } }

        public void Buy(int amount) { throw new System.InvalidOperationException(); }
        public void Sell(int amount) { throw new System.InvalidOperationException(); }

        private ObscuredInt _quantity;
    }

    public class PlayerProduct : IProduct
    {
        public PlayerProduct(PlayerResources playerResources, IItemType type, int quantity = 1, int invertedPriceScale = 2)
        {
            _playerResources = playerResources;
            _invertedPriceScale = invertedPriceScale;
            Type = type;
            Quantity = quantity;
        }

        public IItemType Type { get; private set; }
        public int Quantity { get { return _quantity; } private set { _quantity = value; } }

        public Price Price { get { return Type.Price / _invertedPriceScale; } }

        public void Buy(int amount)
        {
            throw new System.InvalidOperationException();
        }

        public void Sell(int amount)
        {
            amount = amount <= 0 ? Quantity : System.Math.Min(amount, Quantity);

            Quantity -= amount;
            (Price*amount).Consume(_playerResources);
            Type.Withdraw(amount);
        }

        private ObscuredInt _quantity;
        private readonly int _invertedPriceScale;
        private readonly PlayerResources _playerResources;
    }

    public class RandomComponentProduct : IProduct
    {
        public RandomComponentProduct(IDatabase database, ItemTypeFactory factory, ISessionData session, PlayerResources resources, int marketId, int itemId, int itemLevel,
            Constructor.ComponentQuality maxQuality, Faction itemFaction, bool allowRare, long renewalTime, bool premium = false, float priceScale = 2f)
        {
            _factory = factory;
            _session = session;
            _itemId = "component" + itemId;
            _marketId = marketId;
            _playerResources = resources;
            _priceScale = priceScale;

            var purchase = _session.Shop.GetPurchase(marketId, _itemId);
            Quantity = 1 - purchase.CalculateQuantity(renewalTime, System.DateTime.UtcNow.Ticks);

            var random = new System.Random(marketId + 123456789 * itemId + (int)purchase.Time);
            _type = _factory.CreateComponentItem(Constructor.ComponentInfo.CreateRandom(database, itemLevel, itemFaction, random, allowRare, maxQuality), premium);
        }

        public void Buy(int amount)
        {
            if (amount != 1)
                throw new System.InvalidOperationException();

            if (!Price.TryWithdraw(_playerResources))
                throw new System.InvalidOperationException();

            Quantity = 0;
            _session.Shop.SetPurchase(_marketId, _itemId, 1);
            Type.Consume(1);
        }

        public void Sell(int amount) { throw new System.InvalidOperationException(); }

        public IItemType Type { get { return _type; } }

        public int Quantity { get; private set; }
        public Price Price { get { return Type.Price * _priceScale; } }

        private readonly IItemType _type;
        private readonly int _marketId;
        private readonly float _priceScale;
        private readonly string _itemId;
        private readonly ItemTypeFactory _factory;
        private readonly ISessionData _session;
        private readonly PlayerResources _playerResources;
    }

    public class MarketProduct : IProduct
    {
        public MarketProduct(PlayerResources playerResources, IItemType type, int quantity, float priceScale)
        {
            _playerResources = playerResources;
            _quantity = quantity;
            _priceScale = priceScale;
            Type = type;
        }

        public IItemType Type { get; private set; }
        public int Quantity { get { return _quantity - _purchasedCount; } }
        public Price Price { get { return Type.Price * _priceScale; } }

        public void Buy(int amount)
        {
            amount = amount <= 0 ? Quantity : System.Math.Min(amount, Quantity);
            var price = Price * amount;

            if (!price.TryWithdraw(_playerResources))
            	throw new System.InvalidOperationException();

            if (price.Currency != Currency.None)
                _purchasedCount += amount;

            Type.Consume(amount);
        }

        public void Sell(int amount) { throw new System.InvalidOperationException(); }

        private readonly ObscuredInt _quantity;
        private readonly float _priceScale;
        private int _purchasedCount;
        private readonly PlayerResources _playerResources;
    }

    public class ArenaProduct : IProduct
    {
        public ArenaProduct(PlayerResources playerResources, IItemType type, Price price)
        {
            _playerResources = playerResources;
            Type = type;
            Price = price;
        }

        public IItemType Type { get; private set; }
        public int Quantity { get { return 1; } }
        public Price Price { get; private set; }

        public void Buy(int amount)
        {
            var price = Price * amount;

            if (!price.TryWithdraw(_playerResources))
                throw new System.InvalidOperationException();

            Type.Consume(amount);
        }

        public void Sell(int amount) { throw new System.InvalidOperationException(); }

        private readonly PlayerResources _playerResources;
    }

    public class SpecialProduct : IProduct
    {
        public SpecialProduct(PlayerResources playerResources, IItemType type, Price price)
        {
            _playerResources = playerResources;
            Type = type;
            _price = price;
        }

        public IItemType Type { get; private set; }
        public int Quantity { get { return 1; } }
        public Price Price { get { return _price; } }

        public void Buy(int amount)
        {
            if (!_price.TryWithdraw(_playerResources))
                throw new System.InvalidOperationException();

            Type.Consume(1);
        }

        public void Sell(int amount) { throw new System.InvalidOperationException(); }

        private readonly Price _price;
        private readonly PlayerResources _playerResources;
    }

    public class RenewableMarketProduct : IProduct
    {
        public RenewableMarketProduct(PlayerResources playerResources, ISessionData session, IItemType type, int quantity, int marketId, long renewalTime, float priceScale)
        {
            _playerResources = playerResources;
            _quantity = quantity;
            _priceScale = priceScale;
            Type = type;
            _session = session;
            _marketId = marketId;
            var purchase = session.Shop.GetPurchase(marketId, type.Id);
            _purchasedCount = purchase.CalculateQuantity(renewalTime, System.DateTime.UtcNow.Ticks);
        }

        public IItemType Type { get; private set; }
        public int Quantity { get { return _quantity - _purchasedCount; } }
        public Price Price { get { return Type.Price * _priceScale; } }

        public void Buy(int amount)
        {
            amount = amount <= 0 ? Quantity : System.Math.Min(amount, Quantity);
            var price = Price * amount;

            if (!price.TryWithdraw(_playerResources))
            	throw new System.InvalidOperationException();

            _purchasedCount += amount;

            _session.Shop.SetPurchase(_marketId, Type.Id, _purchasedCount);

            Type.Consume(amount);
        }

        public void Sell(int amount) { throw new System.InvalidOperationException(); }

        private readonly ObscuredInt _quantity;
        private readonly float _priceScale;
        private readonly int _marketId;
        private int _purchasedCount;
        private ISessionData _session;
        private readonly PlayerResources _playerResources;
    }

    /*public class ProductForSale
	{
		public ProductForSale(IProduct product)
		{
			Product = product;
		}

		public IProduct Product { get; private set; }

		public int PurchasedCount { get { return _purchased; } }
		public event Action OnPurchasedEvent = () => {};

		public void Buy()
		{
			var player = Game.Data.Player;
			if (Count == 0)
				throw new System.InvalidOperationException("can't buy " + Product.Name);

			switch (Product.Currency)
			{
			case Currency.Credits:
				if (player.Money < Product.Price)
					throw new System.InvalidOperationException("can't buy " + Product.Name);
				player.Money -= Product.Price;
				break;
			}

			PurchasedCount++;

			Product.AddToInventory();
			OnPurchasedEvent();
		}

		public void Remove(int quantity)
		{
			if (PurchasedCount < Product.Count) 
			{
				PurchasedCount += quantity;
			}
			else
			{
				PurchasedCount = Product.Count;
			}
		}

		private int _purchased;
	}*/
}
