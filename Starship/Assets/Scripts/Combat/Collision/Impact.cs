using System;
using Combat.Component.Body;
using GameDatabase.Enums;
using UnityEngine;

namespace Combat.Collision
{
    public class Impulse
    {
        public Impulse()
        {
            _values = new Vector2[8];
            _count = 0;
        }

        public void Apply(IBody body)
        {
            for (var i = 0; i < _count; ++i)
                body.ApplyForce(_values[i*2], _values[i*2 + 1]);
        }

        public void Append(Vector2 position, Vector2 impulse)
        {
            if (_count + 2 >= _values.Length)
                Array.Resize(ref _values, _count + 2);

            _values[_count++] = position;
            _values[_count++] = impulse;
        }

        public Impulse Append(Impulse other)
        {
            if (other == null || other._count == 0)
                return this;

            if (_count + other._count >= _values.Length)
                Array.Resize(ref _values, _count + other._count);

            Array.Copy(other._values, 0, _values, _count, other._count);
            _count += other._count;

            return this;
        }

        public void Clear()
        {
            _count = 0;
        }

        private int _count;
        private Vector2[] _values;
    }

    public struct Impact
    {
        public float KineticDamage;
        public float EnergyDamage;
        public float HeatDamage;
        public float DirectDamage;
        public float Repair;
        public float ShieldDamage;
        public float EnergyDrain;
        public Impulse Impulse;
        public CollisionEffect Effects;

        public float GetTotalDamage(Resistance resistance)
        {
            var minResistance = Mathf.Min(Mathf.Min(resistance.Kinetic, resistance.Energy), resistance.Heat);
            var damage =
                KineticDamage * (1f - resistance.Kinetic) +
                EnergyDamage * (1f - resistance.Energy) +
                HeatDamage * (1f - resistance.Heat) +
                DirectDamage * (1f - 0.5f*resistance.MinResistance);
            return damage;
        }

        public void AddDamage(DamageType type, float amount)
        {
            if (amount < 0)
                throw new InvalidOperationException();

            if (type == DamageType.Direct)
                DirectDamage += amount;
            else if (type == DamageType.Impact)
                KineticDamage += amount;
            else if (type == DamageType.Energy)
                EnergyDamage += amount;
            else if (type == DamageType.Heat)
                HeatDamage += amount;
            else
                throw new System.ArgumentException("unknown damage type");
        }

        public void AddImpulse(Vector2 position, Vector2 impulse)
        {
            if (Impulse == null)
                Impulse = new Impulse();

            Impulse.Append(position, impulse);
        }

        public void ApplyImpulse(IBody body)
        {
            if (Impulse != null)
                Impulse.Apply(body);
        }

        public void RemoveImpulse()
        {
            if (Impulse != null)
                Impulse.Clear();
        }

        public Impact GetDamage(Resistance resistance)
        {
            return new Impact
            {
                KineticDamage = this.KineticDamage * (1f - resistance.Kinetic),
                EnergyDamage = this.EnergyDamage * (1f - resistance.Energy),
                HeatDamage = this.HeatDamage * (1f - resistance.Heat),
                DirectDamage = this.DirectDamage * (1f - 0.5f * resistance.MinResistance),
                ShieldDamage = this.ShieldDamage,
                EnergyDrain = this.EnergyDrain,
                Impulse = this.Impulse,
                Repair = this.Repair,
                Effects = this.Effects
            };
        }

        public void ApplyShield(float power)
        {
            var damage = KineticDamage + EnergyDamage + HeatDamage + DirectDamage;

            if (damage <= 0 || power <= 0)
                return;

            if (damage <= power)
            {
                RemoveDamage();
                ShieldDamage += damage;
            }
            else
            {
                KineticDamage -= power * KineticDamage / damage;
                EnergyDamage -= power * EnergyDamage / damage;
                HeatDamage -= power * HeatDamage / damage;
                DirectDamage -= power * DirectDamage / damage;
                ShieldDamage += power;
            }
        }

        public void RemoveDamage(float amount, Resistance resistance)
        {
            var total = GetTotalDamage(resistance);
            if (total <= amount || total <= 0.000001f)
            {
                RemoveDamage();
                return;
            }

            KineticDamage -= amount * KineticDamage / total;
            EnergyDamage -= amount * EnergyDamage / total;
            HeatDamage -= amount * HeatDamage / total;
            DirectDamage -= amount * DirectDamage / total;
        }

        public void RemoveDamage()
        {
            KineticDamage = 0;
            EnergyDamage = 0;
            HeatDamage = 0;
            DirectDamage = 0;
        }

        public void Append(Impact second)
        {
            KineticDamage += second.KineticDamage;
            EnergyDamage += second.EnergyDamage;
            HeatDamage += second.HeatDamage;
            DirectDamage += second.DirectDamage;
            ShieldDamage += second.ShieldDamage;
            Repair += second.Repair;
            Effects |= second.Effects;
            Impulse = Impulse == null ? second.Impulse : Impulse.Append(second.Impulse);
        }
    }
}
