using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class ShopData : ISerializableData
	{
        [Inject]
		public ShopData(byte[] buffer = null)
		{
			IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "stores";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 3; } }

		public Purchase GetPurchase(int starId, string itemId)
		{				
			Dictionary<string, Purchase> purchases;
			if (!_purchases.TryGetValue(starId, out purchases))
				return new Purchase();

			Purchase purchase;
			if (!purchases.TryGetValue(itemId, out purchase))
				return new Purchase();

			return purchase;
		}

		public void SetPurchase(int starId, string itemId, int quantity)
		{
			IsChanged = true;

			Dictionary<string, Purchase> purchases;
			if (!_purchases.TryGetValue(starId, out purchases))
			{
				purchases = new Dictionary<string, Purchase>();
				_purchases[starId] = purchases;
			}

			purchases[itemId] = new Purchase(quantity, System.DateTime.UtcNow.Ticks);
		}

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;

			foreach (var value in Helpers.Serialize(CurrentVersion))
				yield return value;
			
			foreach (var value in Helpers.Serialize(_purchases.Count))
				yield return value;				
			foreach (var purchaseList in _purchases)
			{
				foreach (var value in Helpers.Serialize(purchaseList.Key))
					yield return value;

				foreach (var value in BitConverter.GetBytes(purchaseList.Value.Count))
					yield return value;
				foreach (var item in purchaseList.Value)
				{
					foreach (var value in Helpers.Serialize(item.Key))
						yield return value;
					foreach (var value in Helpers.Serialize(item.Value.Quantity))
						yield return value;
					foreach (var value in Helpers.Serialize(item.Value.Time))
						yield return value;
				}
			}
		}
		
		private void Deserialize(byte[] buffer)
		{
			if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

			int index = 0;
			var version = Helpers.DeserializeInt(buffer, ref index);
			if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
			{
				OptimizedDebug.Log("ShopData: incorrect data version");
                throw new ArgumentException();
            }

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (var i = 0; i < count; ++i)
			{
				var shopId = Helpers.DeserializeInt(buffer, ref index);

				var purchases = new Dictionary<string, Purchase>();
				var itemCount = Helpers.DeserializeInt(buffer, ref index);
				for (var j = 0; j < itemCount; ++j)
				{
					var itemId = Helpers.DeserializeString(buffer, ref index);
					var quantity = Helpers.DeserializeInt(buffer, ref index);
					var time = Helpers.DeserializeLong(buffer, ref index);
					purchases[itemId] = new Purchase(quantity, time);
				}

				_purchases.Add(shopId, purchases);
			}

            IsChanged = false;
		}

		private static bool TryUpgrade(ref byte[] data, int version)
		{
			if (version == 1)
			{
				data = Upgrade_1_2(data).ToArray();
				version = 2;
			}

            if (version == 2)
            {
                data = Upgrade_2_3(data).ToArray();
                version = 3;
            }

            return version == CurrentVersion;
		}
		
		private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
		{
			int index = 0;
			
			Helpers.DeserializeInt(buffer, ref index);
			var version = 2;
			foreach (var value in Helpers.Serialize(version))
				yield return value;				
			
			foreach (var value in Helpers.Serialize(0)) // purchases
				yield return value;
		}

        private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index);
            var version = 3;
            foreach (var value in Helpers.Serialize(version))
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // purchases
                yield return value;
        }

        private Dictionary<int, Dictionary<string, Purchase>> _purchases = new Dictionary<int, Dictionary<string, Purchase>>();

		public struct Purchase
		{
			public Purchase(int quantity, long time)
			{
				Time = time;
				Quantity = quantity;
			}

			public int CalculateQuantity(long renewalTime, long currentTime)
			{
				if (renewalTime <= 0)
				{
					return Quantity;
				}
				else
				{
					var count = (currentTime - Time) / renewalTime;
					return (int)Math.Max(0L, Quantity - count);
				}
			}

			public readonly long Time;
			public readonly int Quantity;
		}
	}
}
