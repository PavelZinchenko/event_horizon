using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using UnityEngine;

namespace Combat.Factory
{
    public interface IBulletStats
    {
        Component.Systems.Weapons.BulletType Type { get; }
        Component.Systems.Weapons.BulletEffectType EffectType { get; }

        //float Range { get; }
        //float Damage { get; }

        float FlashSize { get; }
        Color FlashColor { get; }
        float FlashTime { get; }

        //float Impulse { get; }
        //float AreaOfEffect { get; }
        //float Velocity { get; }
        float EnergyCost { get; }
        float BulletSpeed { get; }
        float BulletHitRange { get; }
        float Recoil { get; }
        bool IgnoresShipSpeed { get; }

        float PowerLevel { get; set; }
        float RandomFactor { get; set; }
        float HitPointsMultiplier { get; set; }
    }

    public class BulletStats : IBulletStats
    {
        public BulletStats(Ammunition ammunition, WeaponStatModifier statModifier)
        {
            _ammunition = ammunition;
            _statModifier = statModifier;

            PowerLevel = 1.0f;
            RandomFactor = 0.0f;
            HitPointsMultiplier = 1.0f;
        }

        public Component.Systems.Weapons.BulletType Type
        {
            get
            {
                switch (_ammunition.Body.Type)
                {
                    case BulletType.Projectile:
                    case GameDatabase.Enums.BulletType.Static:
                        return Component.Systems.Weapons.BulletType.Projectile;
                    case GameDatabase.Enums.BulletType.Homing:
                        return Component.Systems.Weapons.BulletType.Homing;
                    case GameDatabase.Enums.BulletType.Continuous:
                    default:
                        return Component.Systems.Weapons.BulletType.Direct;
                }
            }
        }

        public Component.Systems.Weapons.BulletEffectType EffectType
        {
            get
            {
                switch (_ammunition.ImpactType)
                {
                    case BulletImpactType.HitFirstTarget:
                    case BulletImpactType.HitAllTargets:
                        return Component.Systems.Weapons.BulletEffectType.Common;
                    case BulletImpactType.DamageOverTime:
                        return Component.Systems.Weapons.BulletEffectType.DamageOverTime;
                    default:
                        return Component.Systems.Weapons.BulletEffectType.Special;
                }
            }
        }

        public float FlashSize { get { return _ammunition.Body.Size * SizeMultiplier; } }
        public Color FlashColor { get { return Color; } }
        public float FlashTime { get { return _ammunition.Body.Type == BulletType.Continuous ? Mathf.Max(0.2f, _ammunition.Body.Lifetime * LifetimeMultiplier) : 0.2f; } }

        public float BulletHitRange { get { return _ammunition.Body.Type == BulletType.Continuous ? Range : Range + BodySize; } }
        public float BulletSpeed { get { return _ammunition.Body.Velocity * _statModifier.VelocityMultiplier.Value; } }
        public float EnergyCost { get { return _ammunition.Body.EnergyCost * _statModifier.EnergyCostMultiplier.Value; } }
        public bool IgnoresShipSpeed { get { return _ammunition.Body.Type == BulletType.Static; } }

        public float Recoil
        {
            get
            {
                if (_ammunition.Body.Type == BulletType.Projectile)
                    return Weight * BulletSpeed;
                if (_ammunition.Body.Type == BulletType.Homing)
                    return Weight * BulletSpeed * 0.1f;
                return 0f;
            }
        }

        public Color Color { get { return _statModifier.Color ?? _ammunition.Body.Color; } }
        public float Weight { get { return _ammunition.Body.Weight * SizeMultiplier * _statModifier.WeightMultiplier.Value; } }

        public float BodySize
        {
            get
            {
                if (IsAoe)
                    return _ammunition.Body.Size * SizeMultiplier * _statModifier.AoeRadiusMultiplier.Value;
                else
                    return _ammunition.Body.Size * SizeMultiplier;
            } 
        }

        public float Range { get { return _ammunition.Body.Range * RangeMultiplier; } }
        public float HitPoints { get { return _ammunition.Body.HitPoints * HitPointsMultiplier; } }
        public float DamageMultiplier { get { return PowerLevel * _statModifier.DamageMultiplier.Value; } }

        //public float Lifetime
        //{
        //    get
        //    {
        //        if (_ammunition.Body.Type == BulletType.Continuous)
        //            return _ammunition.Body.Lifetime * LifetimeMultiplier;
        //        else
        //            return _ammunition.Body.Lifetime * RangeMultiplier;
        //    }
        //}

        public float GetBulletSpeed()
        {
            var velocity = _ammunition.Body.Velocity * VelocityMultiplier;
            return RandomFactor > 0 ? velocity * (1f + (Random.value - 0.5f) * RandomFactor) : velocity;
        }

        //public float GetBulletRange()
        //{
        //    return RandomFactor > 0 ? Range * (1f + (Random.value - 0.5f) * RandomFactor) : Range;
        //}

        public float GetBulletLifetime()
        {
            if (_ammunition.Body.Velocity > 0)
            {
                var range = _ammunition.Body.Range * RangeMultiplier;
                var velocity = _ammunition.Body.Velocity * VelocityMultiplier;
                var lifetime = range / velocity;
                return RandomFactor > 0 ? lifetime * (1f + (Random.value - 0.5f) * RandomFactor) : lifetime;
            }
            else
            {
                return RandomFactor > 0 ? _ammunition.Body.Lifetime * (1f + (Random.value - 0.5f) * RandomFactor) : _ammunition.Body.Lifetime;
            }
        }

        //public float Damage { get; private set; }
        //public float Size { get; private set; }
        //public float Impulse { get; private set; }
        //public float Recoil { get; private set; }
        //public float AreaOfEffect { get; private set; }
        //public float Velocity { get; private set; }

        public float PowerLevel { get; set; }
        public float RandomFactor { get; set; }
        public float HitPointsMultiplier { get; set; }

        private bool IsAoe { get { return (_ammunition.ImpactType == BulletImpactType.DamageOverTime || _ammunition.ImpactType == BulletImpactType.HitAllTargets) && _ammunition.Effects.Count > 0; } }

        private float VelocityMultiplier { get { return _statModifier.VelocityMultiplier.Value; } }
        private float RangeMultiplier { get { return PowerLevel > 0.1f ? PowerLevel * _statModifier.RangeMultiplier.Value : 0f; } }
        private float LifetimeMultiplier { get { return 0.5f + PowerLevel * 0.5f; } }
        private float SizeMultiplier { get { return 0.5f + PowerLevel * 0.5f; } }

        private readonly Ammunition _ammunition;
        private readonly WeaponStatModifier _statModifier;
    }

    public class BulletStatsObsolete : IBulletStats
    {
        public BulletStatsObsolete(AmmunitionObsoleteStats ammunition)
        {
            _stats = ammunition;
            Type = ammunition.AmmunitionClass.GetBulletType();
            EffectType = ammunition.AmmunitionClass.GetEffectType();

            PowerLevel = 1.0f;
            RandomFactor = 0.0f;
            HitPointsMultiplier = 1.0f;
        }

        public Component.Systems.Weapons.BulletType Type { get; private set; }
        public Component.Systems.Weapons.BulletEffectType EffectType { get; private set; }

        public float FlashSize { get { return _stats.Size * SizeMultiplier; } }
        public Color FlashColor { get { return Color; } }
        public float FlashTime { get { return _stats.AmmunitionClass.IsBeam() ? Mathf.Max(0.2f, _stats.LifeTime * LifetimeMultiplier) : 0.2f; } }

        public float Range { get { return _stats.Range * RangeMultiplier; } }
        public float Damage { get { return _stats.Damage * DamageMultiplier; } }
        public float Size { get { return _stats.Size * SizeMultiplier; } }
        public Color Color { get { return _stats.Color; } }
        public float Lifetime { get { return _stats.LifeTime * LifetimeMultiplier; } }
        public float Impulse { get { return _stats.Impulse * SizeMultiplier; } }
        public float Recoil { get { return _stats.Recoil * SizeMultiplier; } }
        public float AreaOfEffect { get { return _stats.AreaOfEffect * SizeMultiplier; } }
        public float Velocity { get { return _stats.Velocity; } }
        public float EnergyCost { get { return _stats.EnergyCost; } }
        public bool IgnoresShipSpeed { get { return _stats.IgnoresShipVelocity; } }

        public float BulletSpeed { get { return Velocity; } }
        public float BulletHitRange { get { return Mathf.Max(Range, AreaOfEffect); } }

        private float RangeMultiplier { get { return PowerLevel > 0.1f ? PowerLevel : 0f; } }
        private float LifetimeMultiplier { get { return 0.5f + PowerLevel * 0.5f; } }
        private float DamageMultiplier { get { return PowerLevel; } }
        private float SizeMultiplier { get { return 0.5f + PowerLevel * 0.5f; } }

        public float PowerLevel { get; set; }
        public float RandomFactor { get; set; }
        public float HitPointsMultiplier { get; set; }

        private readonly AmmunitionObsoleteStats _stats;
    }
}
