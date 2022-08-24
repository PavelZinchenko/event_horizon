using UnityEngine;

namespace GameDatabase.Model
{
    public struct StatMultiplier
    {
        public StatMultiplier(float valueMinusOne)
        {
            _value = valueMinusOne;
        }

        public static StatMultiplier FromValue(float value)
        {
            return new StatMultiplier(value - 1.0f);
        }

        public float Value { get { return 1.0f + _value; } }
        public float Bonus { get { return _value; } }

        public bool HasValue { get { return !Mathf.Approximately(_value, 0); } }

        public static StatMultiplier operator +(StatMultiplier first, StatMultiplier second)
        {
            first._value += second._value;
            return first;
        }

        public static StatMultiplier operator -(StatMultiplier first, StatMultiplier second)
        {
            first._value -= second._value;
            return first;
        }

        public static StatMultiplier operator *(StatMultiplier first, StatMultiplier second)
        {
            first._value = first.Value * second.Value - 1f;
            return first;
        }

        public static StatMultiplier operator +(StatMultiplier first, float modifier)
        {
            first._value += modifier;
            return first;
        }

        public static StatMultiplier operator *(StatMultiplier first, float multiplier)
        {
            first._value = first.Value*multiplier - 1f;
            return first;
        }

        public static StatMultiplier operator %(StatMultiplier first, float multiplier)
        {
            first._value = (first.Value - 1f) * multiplier;
            return first;
        }

        public override string ToString()
        {
            return (_value >= 0 ? "+" : "") + Mathf.RoundToInt(100*_value) + "%";
        }

        private float _value;
    }

}
