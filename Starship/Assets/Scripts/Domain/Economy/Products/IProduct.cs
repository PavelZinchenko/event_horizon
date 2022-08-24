using System.Collections.Generic;
using Economy.ItemType;
using UnityEngine;

namespace Economy.Products
{
    public interface IProduct
    {
        IItemType Type { get; }
        int Quantity { get; }
        Price Price { get; }

        void Buy(int amount = 1);
        void Sell(int amount = 1);
    }

    public static class ProductExtension
    {
        public static void Consume(this IProduct product, int amount = 0)
        {
            if (product == null || product.Type == null)
            {
                var type = product != null ? product.Type : null;
                Debug.LogException(new System.ArgumentException("bad product: " + (type != null ? type.Id : "null")));
                return;
            }

            product.Type.Consume(amount <= 0 ? product.Quantity : Mathf.Min(amount, product.Quantity));
        }

        public static void Withdraw(this IProduct product, int amount = 0)
        {
            product.Type.Withdraw(amount <= 0 ? product.Quantity : Mathf.Min(amount, product.Quantity));
        }

        public static bool IsInPlayerInventory(this IProduct product)
        {
            return product.Type.MaxItemsToWithdraw >= product.Quantity;
        }

        public static void Consume(this IEnumerable<IProduct> products)
        {
            foreach (var item in products)
                item.Consume();
        }
    }
}
