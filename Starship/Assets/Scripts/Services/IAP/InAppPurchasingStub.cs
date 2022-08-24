using System.Collections.Generic;
using System.Linq;

namespace Services.IAP
{
    public class InAppPurchasingsStub : IInAppPurchasing
    {
        public IEnumerable<IIapItem> GetAvailableProducts() { return Enumerable.Empty<IIapItem>(); }

        public void RestorePurchases() {}
        public bool ProcessPurchase(string id) { return false; }
    }
}
