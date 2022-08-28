using System;
using Economy.ItemType;
using Economy.Products;
using GameServices.Player;

namespace Economy
{
    public struct Price
    {
        public Price(long amount, Currency currency)
        {
            if (amount < 0)
                throw new ArgumentException("negative price: " + amount + " " + currency);

            _amount = amount;
            _currency = currency;
        }

        public static Price Common(long amount)
        {
            return new Price(amount, Currency.Credits);
        }

        public static string PriceToString(long amount)
        {
            if (amount < 10000)
                return amount.ToString();
            if (amount < 10000000)
                return amount/1000 + "K";
            else
                return amount/1000000 + "M";
        }

        public static Price Premium(long amount)
        {
            return CurrencyExtensions.PremiumCurrencyAllowed ? new Price(amount, Currency.Stars) : new Price(amount*500, Currency.Credits);
        }

        public static Price Tokens(long amount)
        {
            return new Price(amount, Currency.Tokens);
        }

        public static Price operator *(Price price, int multiplier)
        {
            return new Price(price._amount*multiplier, price._currency);
        }

        public static Price operator *(Price price, float multiplier)
        {
            var value = (long)Math.Ceiling((double) price._amount * multiplier);
            return new Price(value, price._currency);
        }

        public static Price operator /(Price price, int multiplier)
        {
            if (price._amount <= 0)
                return new Price(0, price._currency);

            return new Price(Math.Max(price._amount / multiplier, 1), price._currency);
        }

        public override string ToString()
        {
            return _amount.ToString();
        }

        public int Amount { get { return Clamp(_amount); } }
        public Currency Currency { get { return _currency; } }

        public int GetMaxItemsToWithdraw(PlayerResources playerResources)
        {
            switch (_currency)
            {
                case Currency.Credits:
                    return _amount > 0 ? (int)(playerResources.Money / _amount) : int.MaxValue;
                case Currency.Stars:
                    return _amount > 0 ? (int)(playerResources.Stars / _amount) : int.MaxValue;
                case Currency.Tokens:
                    return _amount > 0 ? (int)(playerResources.Tokens / _amount) : int.MaxValue;
                case Currency.Snowflakes:
                    return _amount > 0 ? (int)(playerResources.Snowflakes / _amount) : int.MaxValue;
                case Currency.None:
                    return 1;
                default:
                    return 0;
            }
        }

        public bool IsEnough(PlayerResources playerResources) { return GetMaxItemsToWithdraw(playerResources) > 0; }

        public void Withdraw(PlayerResources playerResources)
        {
            if (!TryWithdraw(playerResources))
                throw new InvalidOperationException("Price.Withdraw: not enough money - " + Amount + " " + Currency);
        }

        public bool TryWithdraw(PlayerResources playerResources)
        {
            switch (_currency)
            {
                case Currency.Credits:
                    var money = playerResources.Money;
                    if (money < _amount)
                        return false;
                    playerResources.Money = Clamp(money - _amount);
                    return true;
                case Currency.Stars:
                    var stars = playerResources.Stars;
                    if (stars < _amount)
                        return false;
                    playerResources.Stars = Clamp(stars - _amount);
                    return true;
                case Currency.Tokens:
                    var tokens = playerResources.Tokens;
                    if (tokens < _amount)
                        return false;
                    playerResources.Tokens = Clamp(tokens - _amount);
                    return true;
                case Currency.Snowflakes:
                    var snowflakes = playerResources.Snowflakes;
                    if (snowflakes < _amount)
                        return false;
                    playerResources.Snowflakes = Clamp(snowflakes - _amount);
                    return true;
                case Currency.None:
                    return true;
                default:
                    return false;
            }
        }

        public void Consume(PlayerResources playerResources)
        {
            switch (_currency)
            {
                case Currency.Credits:
                    playerResources.Money = Clamp(playerResources.Money + _amount);
                    break;
                case Currency.Stars:
                    playerResources.Stars = Clamp(playerResources.Stars + _amount); ;
                    break;
                case Currency.Tokens:
                    playerResources.Tokens = Clamp(playerResources.Tokens + _amount);
                    break;
                case Currency.Snowflakes:
                    playerResources.Snowflakes = Clamp(playerResources.Snowflakes +_amount);
                    break;
                case Currency.None:
                default:
                    throw new System.ArgumentException();
            }
        }

        public IProduct GetProduct(ItemTypeFactory factory)
        {
            return new Product(factory.CreateCurrencyItem(Currency), Clamp(_amount));
        }

        public static Price ComponentPrice(GameDatabase.DataModel.Component component, float qualityMultiplier = 1.0f)
        {
            var price = 50f + component.Level * 20f;
            if (component.Weapon != null)
                price *= 2f;
            price *= qualityMultiplier;

            return new Price(UnityEngine.Mathf.RoundToInt(price), Currency.Credits);
        }

        public static Price SatellitePrice(GameDatabase.DataModel.Satellite satellite, bool premium = false)
        {
            if (premium)
                return Price.Premium(satellite.Layout.CellCount);
            else
                return Price.Common(satellite.Layout.CellCount*satellite.Layout.CellCount*20);
        }

        public static Price ComponentPremiumPrice(GameDatabase.DataModel.Component component, float qualityMultiplier = 1.0f)
        {
            if (CurrencyExtensions.PremiumCurrencyAllowed)
            {
                var price = qualityMultiplier*component.Level*0.1f;
                if (component.Weapon != null)
                    price *= 1.5f;

                return new Price(1 + UnityEngine.Mathf.RoundToInt(price), Currency.Stars);
            }
            else
            {
                var price = 100f + qualityMultiplier * component.Level * 100f;
                if (component.Weapon != null)
                    price *= 1.5f;

                return new Price(1 + UnityEngine.Mathf.RoundToInt(price), Currency.Credits);
            }
        }

        private static int Clamp(long value) { return value < 0 ? 0 : value > int.MaxValue ? int.MaxValue : (int)value; }

        private readonly ObscuredLong _amount;
        private readonly Currency _currency;
    }
}
