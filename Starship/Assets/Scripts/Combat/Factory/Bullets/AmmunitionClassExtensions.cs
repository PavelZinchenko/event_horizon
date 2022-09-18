using GameDatabase.DataModel;
using GameDatabase.Enums;
using BulletEffectType = Combat.Component.Systems.Weapons.BulletEffectType;
using BulletType = Combat.Component.Systems.Weapons.BulletType;

namespace Combat.Factory
{
    public static class AmmunitionClassExtensions
    {
        public static bool IsBeam(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.EnergyBeam:
                case AmmunitionClassObsolete.EnergySiphon:
                case AmmunitionClassObsolete.VampiricRay:
                case AmmunitionClassObsolete.LaserBeam:
                case AmmunitionClassObsolete.RepairRay:
                case AmmunitionClassObsolete.SmallVampiricRay:
                case AmmunitionClassObsolete.TractorBeam:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsProjectile(this AmmunitionClassObsolete ammunition)
        {
            return !ammunition.IsBeam() && !ammunition.IsAoe();
        }

        public static BulletType GetBulletType(this AmmunitionClassObsolete ammunition)
        {
            if (ammunition.IsBeam())
                return BulletType.Direct;

            switch (ammunition)
            {
                case AmmunitionClassObsolete.EmpMissile:
                case AmmunitionClassObsolete.AcidRocket:
                case AmmunitionClassObsolete.HomingImmobilizer:
                case AmmunitionClassObsolete.HomingTorpedo:
                case AmmunitionClassObsolete.Rocket:
                case AmmunitionClassObsolete.ClusterMissile:
                    return BulletType.Homing;
                case AmmunitionClassObsolete.Aura:
                case AmmunitionClassObsolete.Explosion:
                    return BulletType.AreaOfEffect;
                default:
                    return BulletType.Projectile;
            }
        }

        public static BulletEffectType GetEffectType(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.EnergySiphon:
                case AmmunitionClassObsolete.TractorBeam:
                    return BulletEffectType.Special;
                case AmmunitionClassObsolete.DroneControl:
                    return BulletEffectType.ForDronesOnly;
                case AmmunitionClassObsolete.RepairRay:
                    return BulletEffectType.Repair;
                default:
                    return BulletEffectType.Common;
            }
        }

        public static bool IsDot(this AmmunitionClassObsolete ammunition)
        {
            if (ammunition.IsBeam())
                return true;

            switch (ammunition)
            {
                case AmmunitionClassObsolete.Acid:
                case AmmunitionClassObsolete.Aura:
                case AmmunitionClassObsolete.DamageOverTime:
                case AmmunitionClassObsolete.BlackHole:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsRepair(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.RepairRay:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAoe(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Aura:
                case AmmunitionClassObsolete.Acid:
                case AmmunitionClassObsolete.DamageOverTime:
                case AmmunitionClassObsolete.Explosion:
                    return true;
                default:
                    return false;
            }
        }

        public static bool HasDirectImpulse(this AmmunitionClassObsolete ammunition)
        {
            var type = GetBulletType(ammunition);
            return type != BulletType.Direct && type != BulletType.AreaOfEffect && !IsDot(ammunition);
        }

        public static bool HasDirectDamage(this AmmunitionClassObsolete ammunition, AmmunitionObsoleteStats stats)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Bomb:
                case AmmunitionClassObsolete.Acid:
                case AmmunitionClassObsolete.AcidRocket:
                case AmmunitionClassObsolete.Carrier:
                case AmmunitionClassObsolete.HomingCarrier:
                case AmmunitionClassObsolete.Emp:
                case AmmunitionClassObsolete.EmpMissile:
                case AmmunitionClassObsolete.Fireworks:
                case AmmunitionClassObsolete.UnguidedRocket:
                case AmmunitionClassObsolete.Rocket:
                case AmmunitionClassObsolete.FragBomb:
                case AmmunitionClassObsolete.BlackHole:
                case AmmunitionClassObsolete.ClusterMissile:
                    return false;
                case AmmunitionClassObsolete.IonBeam:
                    return stats.AreaOfEffect <= 0;
                default:
                    return true;
            }
        }

        public static bool StickToTarget(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.EnergyBeam:
                case AmmunitionClassObsolete.EnergySiphon:
                case AmmunitionClassObsolete.VampiricRay:
                case AmmunitionClassObsolete.SmallVampiricRay:
                //case AmmunitionClass.TractorBeam:
                    return true;
                default:
                    return false;
            }
        }

        public static bool HasEngine(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Rocket:
                case AmmunitionClassObsolete.AcidRocket:
                case AmmunitionClassObsolete.HomingTorpedo:
                case AmmunitionClassObsolete.HomingImmobilizer:
                case AmmunitionClassObsolete.EmpMissile:
                case AmmunitionClassObsolete.UnguidedRocket:
                case AmmunitionClassObsolete.ClusterMissile:
                case AmmunitionClassObsolete.HomingCarrier:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsHoming(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Rocket:
                case AmmunitionClassObsolete.AcidRocket:
                case AmmunitionClassObsolete.HomingTorpedo:
                case AmmunitionClassObsolete.HomingImmobilizer:
                case AmmunitionClassObsolete.EmpMissile:
                case AmmunitionClassObsolete.ClusterMissile:
                case AmmunitionClassObsolete.HomingCarrier:
                    return true;
                default:
                    return false;
            }
        }

        //public static int MaxHits(this AmmunitionClass ammunition)
        //{
        //    if (ammunition.IsDot())
        //        return 0;

        //    switch (ammunition)
        //    {
        //        default:
        //            return 1;
        //    }
        //}

        public static bool IsBoundToCannon(this AmmunitionClassObsolete ammunition)
        {
            if (ammunition.IsBeam())
                return true;

            switch (ammunition)
            {
                case AmmunitionClassObsolete.Aura:
                case AmmunitionClassObsolete.Explosion:
                    return true;
                default:
                    return false;
            }
        }

        public static bool ExplodeIfDetonated(this AmmunitionClassObsolete ammunition, AmmunitionObsoleteStats stats)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Bomb:
                case AmmunitionClassObsolete.Rocket:
                case AmmunitionClassObsolete.UnguidedRocket:
                //case AmmunitionClass.BlackHole:
                    return true;
                case AmmunitionClassObsolete.IonBeam:
                    return stats.AreaOfEffect > 0;
                default:
                    return false;
            }
        }

        public static bool AoeIfDetonated(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.AcidRocket:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanHitAllies(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.RepairRay:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanBeDisarmed(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Acid:
                case AmmunitionClassObsolete.Aura:
                case AmmunitionClassObsolete.BlackHole:
                case AmmunitionClassObsolete.DamageOverTime:
                case AmmunitionClassObsolete.EnergyBeam:
                case AmmunitionClassObsolete.EnergySiphon:
                case AmmunitionClassObsolete.Explosion:
                case AmmunitionClassObsolete.IonBeam:
                case AmmunitionClassObsolete.VampiricRay:
                case AmmunitionClassObsolete.LaserBeam:
                case AmmunitionClassObsolete.SmallVampiricRay:
                case AmmunitionClassObsolete.TractorBeam:
                case AmmunitionClassObsolete.RepairRay:
                    return false;
                default:
                    return true;
            }
        }

        public static bool EmpIfDetonated(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Emp:
                case AmmunitionClassObsolete.EmpMissile:
                    return true;
                default:
                    return false;
            }
        }

        public static bool WebIfDetonated(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.PlasmaWeb:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsCarrier(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Carrier:
                case AmmunitionClassObsolete.HomingCarrier:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsClusterBomb(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.FragBomb:
                case AmmunitionClassObsolete.ClusterMissile:
                    return true;
                default:
                    return false;
            }
        }

        public static bool DetonateIfExpired(this AmmunitionClassObsolete ammunition)
        {
            switch (ammunition)
            {
                case AmmunitionClassObsolete.Bomb:
                case AmmunitionClassObsolete.Carrier:
                case AmmunitionClassObsolete.Rocket:
                case AmmunitionClassObsolete.AcidRocket:
                case AmmunitionClassObsolete.UnguidedRocket:
                case AmmunitionClassObsolete.Emp:
                case AmmunitionClassObsolete.EmpMissile:
                case AmmunitionClassObsolete.FragBomb:
                case AmmunitionClassObsolete.Fireworks:
                case AmmunitionClassObsolete.PlasmaWeb:
                case AmmunitionClassObsolete.ClusterMissile:
                    return true;
                default:
                    return false;
            }
        }
    }
}
