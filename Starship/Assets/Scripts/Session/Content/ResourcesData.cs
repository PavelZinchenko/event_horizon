using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class ResourcesData : ISerializableData
	{
        [Inject]
        public ResourcesData(
            FuelValueChangedSignal.Trigger fuelValueChangedTrigger, 
            MoneyValueChangedSignal.Trigger moneyValueChangedTrigger, 
            StarsValueChangedSignal.Trigger starsValueChangedTrigger, 
            TokensValueChangedSignal.Trigger tokensValueChangedTrigger,
            ResourcesChangedSignal.Trigger specialResourcesChangedTrigger,
            byte[] buffer = null)
        {
            _moneyValueChangedTrigger = moneyValueChangedTrigger;
            _fuelValueChangedTrigger = fuelValueChangedTrigger;
            _starsValueChangedTrigger = starsValueChangedTrigger;
            _tokensValueChangedTrigger = tokensValueChangedTrigger;
            _specialResourcesChangedTrigger = specialResourcesChangedTrigger;

            IsChanged = true;
			_money = 100;
			_stars = 0;
            _tokens = 0;
			_fuel = GameServices.Player.MotherShip.FuelMinimum;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);

            _resources.CollectionChangedEvent += OnCollectionChanged;
        }

        public string FileName { get { return Name; } }
        public const string Name = "resources";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 5; } }

		public int Money 
		{
			get { return _money; }
			set
			{
                if (_money == value)
                    return;

			    IsChanged = true;
				_money = value; 
                _moneyValueChangedTrigger.Fire(_money);
			}
		}
		
		public int Fuel
		{
			get { return _fuel; }
			set
			{
                if (_fuel == value)
                    return;

				IsChanged = true;
				_fuel = value;
                _fuelValueChangedTrigger.Fire(_fuel);
			}
		}

        public int Tokens
        {
            get { return _tokens; }
            set
            {
                if (_tokens == value)
                    return;

                IsChanged = true;
                _tokens = value;
                _tokensValueChangedTrigger.Fire(_tokens);
            }
        }

        public int Stars
		{
			get { return _stars; }
			set
			{
                if (_stars == value)
                    return;

			    IsChanged = true;
				_stars = value; 
                _starsValueChangedTrigger.Fire();
			}
		}

	    public IGameItemCollection<int> Resources { get { return _resources; } }

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;

			foreach (var value in Helpers.Serialize(Money))
				yield return value;
			foreach (var value in Helpers.Serialize(Fuel))
				yield return value;
			foreach (var value in Helpers.Serialize(Stars))
				yield return value;
            foreach (var value in Helpers.Serialize(Tokens))
                yield return value;
		    foreach (var value in Helpers.Serialize(_resources))
		        yield return value;

		    foreach (var value in Helpers.Serialize(0)) // reserved
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
        }

        private void Deserialize(byte[] buffer)
		{
			if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

			int index = 0;
			var version = Helpers.DeserializeInt(buffer, ref index);
			if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
			{
				UnityEngine.Debug.Log("ResourcesData: incorrect data version");
                throw new ArgumentException();
            }

			_money = Helpers.DeserializeInt(buffer, ref index);
			_fuel = Helpers.DeserializeInt(buffer, ref index);
			_stars = Helpers.DeserializeInt(buffer, ref index);
            _tokens = Helpers.DeserializeInt(buffer, ref index);
		    _resources.Assign(Helpers.DeserializeDictionary(buffer, ref index));

#if UNITY_EDITOR
			UnityEngine.Debug.Log("ResourcesData: money = " + _money);
			UnityEngine.Debug.Log("ResourcesData: fuel = " + _fuel);
			UnityEngine.Debug.Log("ResourcesData: stars = " + _stars);
            UnityEngine.Debug.Log("ResourcesData: tokens = " + _tokens);
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

            if (version == 3)
            {
                data = Upgrade_3_4(data).ToArray();
                version = 4;
            }

		    if (version == 4)
		    {
		        data = Upgrade_4_5(data).ToArray();
		        version = 5;
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

			for (int i = index; i < buffer.Length; ++i)
				yield return buffer[i];

			foreach (var value in Helpers.Serialize(0)) // commonResourcesCount
				yield return value;
		}

		private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
		{
			int index = 0;

			Helpers.DeserializeInt(buffer, ref index);
			var version = 3;
			foreach (var value in Helpers.Serialize(version))
				yield return value;				

			var money = Helpers.DeserializeInt(buffer, ref index);
			var fuel = Helpers.DeserializeInt(buffer, ref index);
			var stars = Helpers.DeserializeInt(buffer, ref index);

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeInt(buffer, ref index);
				stars += (value+4)/5;
			}

			foreach (var value in Helpers.Serialize(money))
				yield return value;				
			foreach (var value in Helpers.Serialize(fuel))
				yield return value;
			foreach (var value in Helpers.Serialize(stars))
				yield return value;				

			for (int i = index; i < buffer.Length; ++i)
				yield return buffer[i];
		}

        private static IEnumerable<byte> Upgrade_3_4(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index); // version
            foreach (var value in Helpers.Serialize(4))
                yield return value;

            var money = Helpers.DeserializeInt(buffer, ref index);
            var fuel = Helpers.DeserializeInt(buffer, ref index);
            var stars = Helpers.DeserializeInt(buffer, ref index);

            foreach (var value in Helpers.Serialize(money))
                yield return value;
            foreach (var value in Helpers.Serialize(fuel))
                yield return value;
            foreach (var value in Helpers.Serialize(stars))
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // tokens
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;

            for (int i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

	    private static IEnumerable<byte> Upgrade_4_5(byte[] buffer)
	    {
	        int index = 0;

	        Helpers.DeserializeInt(buffer, ref index); // version
	        foreach (var value in Helpers.Serialize(5))
	            yield return value;

	        var money = Helpers.DeserializeInt(buffer, ref index);
	        var fuel = Helpers.DeserializeInt(buffer, ref index);
	        var stars = Helpers.DeserializeInt(buffer, ref index);
	        var tokens = Helpers.DeserializeInt(buffer, ref index);

	        foreach (var value in Helpers.Serialize(money))
	            yield return value;
	        foreach (var value in Helpers.Serialize(fuel))
	            yield return value;
	        foreach (var value in Helpers.Serialize(stars))
	            yield return value;
	        foreach (var value in Helpers.Serialize(tokens))
	            yield return value;

	        var resources = Helpers.DeserializeDictionary(buffer, ref index);
	        var commonResources = Helpers.DeserializeDictionary(buffer, ref index);

	        foreach (var item in commonResources)
	        {
	            int value;
                if (resources.TryGetValue(item.Key, out value))
                    resources[item.Key] = value + item.Value;
                else
                    resources.Add(item.Key, item.Value);
	        }

	        foreach (var value in Helpers.Serialize(resources))
	            yield return value;

	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
        }

        private void OnCollectionChanged()
	    {
	        IsChanged = true;
	        _specialResourcesChangedTrigger.Fire();
	    }

        private ObscuredInt _money;
		private ObscuredInt _fuel;
		private ObscuredInt _stars;
        private ObscuredInt _tokens;
		private readonly Dictionary<int, int> _commonResources = new Dictionary<int, int>();
	    private readonly GameItemCollection<int> _resources = new GameItemCollection<int>();

        private readonly FuelValueChangedSignal.Trigger _fuelValueChangedTrigger;
        private readonly MoneyValueChangedSignal.Trigger _moneyValueChangedTrigger;
        private readonly StarsValueChangedSignal.Trigger _starsValueChangedTrigger;
        private readonly TokensValueChangedSignal.Trigger _tokensValueChangedTrigger;
	    private readonly ResourcesChangedSignal.Trigger _specialResourcesChangedTrigger;

        private static readonly int _mask = new System.Random((int)DateTime.Now.Ticks).Next();
	}

    public class MoneyValueChangedSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
    public class FuelValueChangedSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
    public class StarsValueChangedSignal : SmartWeakSignal { public class Trigger : TriggerBase { } }
    public class TokensValueChangedSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
    public class ResourcesChangedSignal : SmartWeakSignal { public class Trigger : TriggerBase { } }
}
