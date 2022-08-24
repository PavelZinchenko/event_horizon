using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using Economy;
using Economy.Products;
using GameDatabase.DataModel;
using GameDatabase.Model;
using Services.Localization;
using Services.Reources;

namespace DataModel.Technology
{
    public enum CraftItemQuality
    {
        Common,
        Improved,
        Excellent,
        Superior,
    }

    public static class CraftItemQualityExtensions
    {
        public static int GetWorkshopLevel(this CraftItemQuality quality, int defaultLevel)
        {
            switch (quality)
            {
                case CraftItemQuality.Common:
                    return defaultLevel;
                case CraftItemQuality.Improved:
                    return defaultLevel + Mathf.Max(1, defaultLevel/10);
                case CraftItemQuality.Excellent:
                    return defaultLevel + Mathf.Max(3, defaultLevel/7);
                case CraftItemQuality.Superior:
                    return defaultLevel + Mathf.Max(5, defaultLevel/5);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }

    public struct CraftingPrice
    {
        public CraftingPrice(int credits, int stars = 0, int techs = 0)
        {
            Credits = credits;
            Techs = techs;

            if (CurrencyExtensions.PremiumCurrencyAllowed)
            {
                Stars = stars;
            }
            else
            {
                Stars = 0;
                Credits += Price.Premium(Stars).Amount;
            }
        }

        public static CraftingPrice operator +(CraftingPrice first, CraftingPrice second)
        {
            return new CraftingPrice(first.Credits + second.Credits, first.Stars + second.Stars, first.Techs + second.Techs);
        }

        public static CraftingPrice operator -(CraftingPrice first, CraftingPrice second)
        {
            return new CraftingPrice(first.Credits - second.Credits, first.Stars - second.Stars, first.Techs - second.Techs);
        }

        public static CraftingPrice operator *(CraftingPrice price, float scale)
        {
            return new CraftingPrice(Mathf.RoundToInt(price.Credits*scale), Mathf.RoundToInt(price.Stars*scale), Mathf.RoundToInt(price.Techs*scale));
        }

        public readonly int Credits;
        public readonly int Stars;
        public readonly int Techs;
    }


	public interface ITechnology
	{
        ItemId<GameDatabase.DataModel.Technology> Id { get; }
	    string GetName(ILocalization localization);
	    Sprite GetImage(IResourceLocator resourceLocator);
	    string GetDescription(ILocalization localization);
        Color Color { get; }
		Faction Faction { get; }
		int Price { get; }
		bool Hidden { get; }
        bool Special { get; }

	    CraftingPrice GetCraftPrice(CraftItemQuality quality);
		IProduct CreateItem(CraftItemQuality quality = CraftItemQuality.Common, System.Random random = null);

		IEnumerable<ITechnology> Requirements { get; }
	}
}
