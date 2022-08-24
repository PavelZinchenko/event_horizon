using GameDatabase.Enums;

namespace Constructor.Modification
{
  //  public enum ModificationType // Keep order!
  //  {
  //      Default,
  //      Lightweight,
  //      LowEnergyCost,
  //      Fortified,
  //      ExtraHitPoints,
  //      Damage,
  //      Cooldown,
  //      Range,
  //      BulletVelocity,
		//EnergyCapacity,
		//RepairRate,
		//EnginePower,
		//RechargeRate,
  //      BulletVelocity2,
  //      AreaOfEffect,
  //      ShieldPower,
  //      Damage2,
  //      DroneDamage,
  //      DroneDefense,
  //      DroneSpeed,
  //      DroneRange,
  //      Recoil,
  //  }

    public static class ModificationTypeExtension
    {
        public static IModification Create(this ComponentModType type, ModificationQuality quality)
        {
            switch (type)
            {
            case ComponentModType.Lightweight:
                return new Lightweight(quality);
            case ComponentModType.LowEnergyCost:
                return new LowEnergyCost(quality);
            case ComponentModType.Fortified:
                return new Fortified(quality);
            case ComponentModType.ExtraHitPoints:
                return new ExtraHitPoints(quality);
            case ComponentModType.Damage:
                return new Damage(quality);
            case ComponentModType.Damage2:
                return new Damage2(quality);
            case ComponentModType.Cooldown:
                return new Cooldown(quality);
            case ComponentModType.Range:
                return new Range(quality);
            case ComponentModType.BulletVelocity:
                return new BulletVelocity(quality);
            case ComponentModType.BulletVelocity2:
                return new BulletVelocity2(quality);
            case ComponentModType.EnergyCapacity:
				return new EnergyCapacity(quality);
			case ComponentModType.RepairRate:
				return new RepairRate(quality);
			case ComponentModType.EnginePower:
				return new EnginePower(quality);
			case ComponentModType.RechargeRate:
				return new RechargeRate(quality);
            case ComponentModType.AreaOfEffect:
                return new AreaOfEffect(quality);
            case ComponentModType.ShieldPower:
                return new ShieldPower(quality);
            case ComponentModType.DroneDamage:
                return new DroneDamage(quality);
            case ComponentModType.DroneDefense:
                return new DroneDefense(quality);
            case ComponentModType.DroneSpeed:
                return new DroneSpeed(quality);
            case ComponentModType.DroneRange:
                return new DroneRange(quality);
            case ComponentModType.Recoil:
                return new Recoil(quality);
            default:
                return null;
            }
        }
    }
}
