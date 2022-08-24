using System.Collections.Generic;
using System.Linq;
using Economy.ItemType;
using Economy.Products;
using Game;
using Services.Ads;
using Services.IAP;
using Services.Social;

namespace GameModel.Quests
{
    public class IapInventory : IInventory
    {
        public IapInventory(ProductFactory productFactory, IInAppPurchasing iapPurchasing, IAdsManager adsManager, IFacebookFacade facebookFacade, ItemTypeFactory itemTypeFactory, HolidayManager holidayManager)
        {
            _productFactory = productFactory;
            _inAppPurchasing = iapPurchasing;
            _adsManager = adsManager;
            _facebookFacade = facebookFacade;
            _itemTypeFactory = itemTypeFactory;
            _holidayManager = holidayManager;
        }

        public void Refresh() {}

        public IEnumerable<IProduct> Items
        {
            get
            {
                var items = new List<IProduct>(_inAppPurchasing.GetAvailableProducts().Select(item => _productFactory.CreateMarketProduct(item, 1, 0)));

                if (_holidayManager.IsChristmas)
                    items.Add(_productFactory.CreateMarketProduct(_itemTypeFactory.CreateXmasBoxItem()));

                if (_facebookFacade.IsRewardedPostAvailable)
                    items.Add(_productFactory.CreateMarketProduct(_itemTypeFactory.CreateRewardedFacebookPostItem()));

                if (_adsManager.Status != Status.NotInitialized)
                    items.Add(_productFactory.CreateMarketProduct(_itemTypeFactory.CreateRewardedAdItem()));

                return items;
            }
        }

        private readonly IInAppPurchasing _inAppPurchasing;
        private readonly ProductFactory _productFactory;
        private readonly IAdsManager _adsManager;
        private readonly IFacebookFacade _facebookFacade;
        private readonly ItemTypeFactory _itemTypeFactory;
        private readonly HolidayManager _holidayManager;
    }
}
