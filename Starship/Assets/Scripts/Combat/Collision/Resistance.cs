using UnityEngine;

namespace Combat.Collision
{
    public struct Resistance
    {
        public float Kinetic;
        public float Energy;
        public float Heat;
        public float EnergyDrain;

        public float MinResistance { get { return Mathf.Min(Mathf.Min(Kinetic, Energy), Heat); } }

        public static readonly Resistance Empty = new Resistance();
    }
}
