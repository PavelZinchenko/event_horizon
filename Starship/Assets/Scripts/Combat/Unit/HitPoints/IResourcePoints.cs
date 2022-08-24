using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Combat.Unit.HitPoints
{
	public interface IResourcePoints
	{
		float Value { get; }
		float MaxValue { get; }
		float Percentage { get; }
		float RechargeRate { get; }

		void Update(float elapsedTime);
		//IEnumerable<byte> Serialize();
		//void Deserialize(byte[] data, ref int index);

        bool Exists { get; }

		bool TryGet(float value);
		void Get(float value);
	}

    public class EmptyResources : IResourcePoints
    {
        public float Value { get { return 0.0f; } }
        public float MaxValue { get { return 1.0f; } }
        public float Percentage { get { return 0.0f; } }
        public float RechargeRate { get { return 0.0f; } }
        public void Update(float elapsedTime) { }
        //public IEnumerable<byte> Serialize() { return Enumerable.Empty<byte>(); }
        //public void Deserialize(byte[] data, ref int index) { }
        public bool TryGet(float value) { return true; }
        public void Get(float value) { }
        public bool Exists { get { return false; } }
    }

    public class UnlimitedEnergy : IResourcePoints
	{
		public float Value { get { return 100f; } }
		public float MaxValue { get { return 100f; } }
		public float Percentage { get { return 1.0f; } }		
		public float RechargeRate { get { return 0.0f; } }		
		public void Update(float elapsedTime) {}
		public IEnumerable<byte> Serialize() { return Enumerable.Empty<byte>(); }
		public void Deserialize(byte[] data, ref int index) {}
		public bool TryGet(float value) { return true; }
		public void Get(float value) {}
        public bool Exists { get { return true; } }
    }

	public class Energy : IResourcePoints
	{
		public Energy(float max, float rechargeRate, float rechargeDelay)
		{
			_rechargeRate = rechargeRate;
			MaxValue = max;
			_rechargeDelay = rechargeDelay;
			_value = 1.0f;
			_delay = 0.0f;
		}
		
		public float Value { get { return MaxValue*_value; } }
		public float Percentage { get { return _value; } }
		public float MaxValue { get; private set; }
		public float RechargeRate { get { return _rechargeRate; } }

		public void Update(float elapsedTime)
		{
			if (_delay > elapsedTime)
			{
				_delay -= elapsedTime;
				return;
			}

            if (MaxValue > 0)
			    ThreadSafe.AddClamp(ref _value, (elapsedTime - _delay)*_rechargeRate/MaxValue, 0, 1);
			_delay = 0;
		}
		
		public bool TryGet(float how)
		{
			ThreadSafe.Function<float> func = (ref float value) =>
			{
				if (value > 0 && value*MaxValue >= how && MaxValue > 0)
				{
					value -= how/MaxValue;
					return true;
				}
				return false;
			};

			if (ThreadSafe.ChangeValue(ref _value, func))
			{
				_delay = _rechargeDelay;
				return true;
			}

			return false;
		}

		public void Get(float how)
		{
            if (MaxValue > 0)
			    ThreadSafe.AddClamp(ref _value, -how/MaxValue, 0, 1);

			if (how > 0)
				_delay = _rechargeDelay;
		}
		
		public readonly static Energy Zero = new Energy(0,1,1);

		//public IEnumerable<byte> Serialize()
		//{
		//	foreach (var value in Helpers.Serialize(_value))
		//		yield return value;
		//	foreach (var value in Helpers.Serialize(_delay))
		//		yield return value;
		//}

		//public void Deserialize(byte[] data, ref int index)
		//{
		//	_value = Helpers.DeserializeFloat(data, ref index);
		//	_delay = Helpers.DeserializeFloat(data, ref index);
		//}

	    public bool Exists { get { return MaxValue > 0; } }

	    public static int SerializedSize { get { return 2*sizeof(float); } }

		private float _value;
		private float _delay;
		
		private readonly float _rechargeRate;
		private readonly float _rechargeDelay;
	}

	public class HitPoints : IResourcePoints
	{
		public HitPoints(float max)
		{
			_value = -1.0f;
			_max = Mathf.Max(max, 1);
		}
		
		public float Value { get { return -_value*_max; } }
		public float MaxValue { get { return _max; } }
		public float Percentage { get { return -_value; } }
		public float RechargeRate { get { return 0; } }

		public void Update(float elapsedTime) {}

		public void Get(float how)
		{
			ThreadSafe.AddClamp(ref _value, how/_max, -1, 0);
		}
		
		public bool TryGet(float how)
		{
			throw new System.NotImplementedException();
		}

		//public IEnumerable<byte> Serialize() { return Helpers.Serialize(_value); }
		//public void Deserialize(byte[] data, ref int index) { _value = Helpers.DeserializeFloat(data, ref index); }
		public static int SerializedSize { get { return sizeof(float); } }
        public bool Exists { get { return true; } }

        private float _value;
		private readonly float _max;
	}
}
