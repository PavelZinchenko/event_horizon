using UnityEngine;
using System.Threading;

public static class ThreadSafe
{
	public static void Add(ref float target, float value)
	{
		float newCurrentValue = target;
		while (true)
		{
			var currentValue = newCurrentValue;
			var newValue = currentValue + value;
			newCurrentValue = Interlocked.CompareExchange(ref target, newValue, currentValue);

			if (newCurrentValue == currentValue)
				break;
		}
	}

	public static void AddClamp(ref float target, float value, float min, float max)
	{
		float newCurrentValue = target;
		while (true)
		{
			var currentValue = newCurrentValue;

			var newValue = currentValue + value;
			if (newValue < min) newValue = min;
			if (newValue > max) newValue = max;

			newCurrentValue = Interlocked.CompareExchange(ref target, newValue, currentValue);

			if (newCurrentValue == currentValue)
				break;
		}
	}

	public delegate bool Function<T>(ref T value);

	public static bool ChangeValue(ref float target, Function<float> function)
	{
		float newCurrentValue = target;
		while (true)
		{
			var currentValue = newCurrentValue;
			var newValue = currentValue;

			if (!function(ref newValue))
				return false;

			newCurrentValue = Interlocked.CompareExchange(ref target, newValue, currentValue);

			if (newCurrentValue == currentValue)
				return true;
		}
	}
}
