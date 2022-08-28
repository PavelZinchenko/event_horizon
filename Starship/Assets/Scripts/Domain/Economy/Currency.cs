using System.ComponentModel;
using GameModel;

namespace Economy
{
    public enum Currency
    {
        None,
        Credits,
        Stars,
        Tokens,
        Snowflakes,
    }

    public static class CurrencyExtensions
    {
        //public static Currency CommonCurrency { get { return Currency.Credits; } }
        //public static IProductType CommonCurrencyItem { get { return _creditsType; } }
        //public static IProduct GetCommonCurrency(int amount) { return new Product(_creditsType, amount); }

#if UNITY_PURCHASING && !UNITY_STANDALONE
        public static bool PremiumCurrencyAllowed => true;
#else
        public static bool PremiumCurrencyAllowed => false;
#endif

        //public static IProductType ProductType(this Currency currency)
        //{
        //    switch (currency)
        //    {
        //        case Currency.Credits:
        //            return _creditsType;
        //        case Currency.Crystals:
        //            return _starsType;
        //        case Currency.Money:
        //        default:
        //            throw new InvalidEnumArgumentException();
        //    }
        //}

        //private static readonly IProductType _creditsType = new MoneyItem();
        //private static readonly IProductType _starsType = new CrystalsItem();
    }
}
