using System.Collections.Generic;
using Economy.ItemType;
using Utils;

namespace Services.IAP
{
	public interface IInAppPurchasing
	{
		IEnumerable<IIapItem> GetAvailableProducts();
        void RestorePurchases();
	    bool ProcessPurchase(string id);
	}

	public interface IIapItem : IItemType
	{
		string PriceText { get; }
	}

    public static class ProductIds
    {
        public const string StarPack_20_Id = "eventhorizon.star_pack_20";
        public const string StarPack_50_Id = "eventhorizon.star_pack_50";
        public const string StarPack_100_Id = "eventhorizon.star_pack_100";
        public const string StarPack_250_Id = "eventhorizon.star_pack_250";
        public const string StarPack_500_Id = "eventhorizon.star_pack_500";
        public const string StarPack_1000_Id = "eventhorizon.star_pack_1000";
        public const string StarPack_2000_Id = "eventhorizon.star_pack_2000";
        //public const string StarPack_Id = "eventhorizon.star_pack";
        //public const string StarPack2_Id = "eventhorizon.star_pack2";
        //public const string StarPack3_Id = "eventhorizon.star_pack3";
        //public const string RemoveAds_Id = "eventhorizon.remove_ads";
        public const string SupporterPack_Id = "eventhorizon.supporter_pack";
    }

    public class InAppPurchaseCompletedSignal : SmartWeakSignal { public class Trigger : TriggerBase { } }
    public class InAppPurchaseFailedSignal : SmartWeakSignal<string> { public class Trigger : TriggerBase { } }
}
