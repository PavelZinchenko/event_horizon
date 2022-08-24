using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Zenject;

namespace Session.Content
{
    public class InAppPurchasesData : ISerializableData
	{
        [Inject]
		public InAppPurchasesData(StarsValueChangedSignal.Trigger starsValueChangedTrigger, byte[] buffer = null)
        {
            _starsValueChangedTrigger = starsValueChangedTrigger;

            IsChanged = true;
			_purchasedStars = 0;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "iap";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 3; } }

		public bool RemoveAds
		{
			get { return _removeAds || _supporterPack || _purchasedStars > 0; }
			set
			{
				IsChanged |= _removeAds != value;
				_removeAds = value;
			}
		}
		
		public bool SupporterPack
		{
			get { return _supporterPack; }
			set
			{
                if (_supporterPack == value)
                    return;

				IsChanged = true;
				_supporterPack = value;
                _starsValueChangedTrigger.Fire();
			}
		}

		public int PurchasedStars 
		{
			get { return _purchasedStars; }
			set
			{
				IsChanged |= _purchasedStars != value;

				if (_shouldUpgradeToV3)
				{
					IsChanged = true;
					_purchasedStars = 0;
					_shouldUpgradeToV3 = false;
				}

				_purchasedStars = value;
			}
		}

        public int TotalPurchasedStars
        {
            get
            {
                var stars = PurchasedStars;
                if (_supporterPack)
                    stars += 100;

                return stars;
            }
        }

		public int ExtraStarCount
		{
			get
			{
				var count = _supporterPack ? 100 : 0;
				if (_shouldUpgradeToV3)
					count += _purchasedStars;

				return count;
			}
		}

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;

			yield return _removeAds ? (byte)1 : (byte)0;
			yield return _supporterPack ? (byte)1 : (byte)0;
			foreach (var value in Helpers.Serialize(_purchasedStars))
				yield return value;
			yield return _shouldUpgradeToV3 ? (byte)1 : (byte)0;
		}
		
		private void Deserialize(byte[] buffer)
		{
			if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

			int index = 0;
			var version = Helpers.DeserializeInt(buffer, ref index);
			if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
			{
				UnityEngine.Debug.Log("InAppPurchasesData: incorrect data version");
                throw new ArgumentException();
            }

			_removeAds = buffer[index++] != 0;
			_supporterPack = buffer[index++] != 0;
			_purchasedStars = Math.Max(0, Helpers.DeserializeInt(buffer, ref index));
			_shouldUpgradeToV3 = buffer[index++] != 0;

#if UNITY_EDITOR
			UnityEngine.Debug.Log("InAppPurchasesData: removeads = " + _removeAds);
			UnityEngine.Debug.Log("InAppPurchasesData: fuel = " + _supporterPack);
			UnityEngine.Debug.Log("InAppPurchasesData: stars = " + _purchasedStars);
#endif

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
			UnityEngine.Debug.Log("InAppPurchasesData.Upgrade_1_2");
			
			int index = 0;
			
			Helpers.DeserializeInt(buffer, ref index);
			var version = 2;
			foreach (var value in Helpers.Serialize(version))
				yield return value;				
			
			var removeAds = buffer[index++] != 0;
			yield return removeAds ? (byte)1 : (byte)0;
			yield return 0;

			foreach (var value in Helpers.Serialize(0))
				yield return value;
		}

		private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
		{
			UnityEngine.Debug.Log("InAppPurchasesData.Upgrade_2_3");
			foreach (var value in buffer)
				yield return value;

			yield return 1;
		}

		private bool _removeAds;
		private bool _supporterPack;
		private bool _shouldUpgradeToV3;
		private ObscuredInt _purchasedStars;

        private readonly StarsValueChangedSignal.Trigger _starsValueChangedTrigger;
	}
}
