using Combat.Component.Ship;
using Combat.Component.Systems.Weapons;

namespace Combat.Ai
{
	public static class ActionFactory
	{
		public static IAction CreateAttackAction(IShip ship, int weaponIndex, int level)
		{
			var weapon = ship.Systems.All.Weapon(weaponIndex);
			
			switch (weapon.Info.BulletType)
			{
			case BulletType.Projectile:
                if (weapon.Info.WeaponType == WeaponType.Manageable)
                    return new ProjectileAttackAction(weaponIndex);
                return level < 100 ? (IAction)(new DirectAttackAction(weaponIndex)) : (IAction)(new ProjectileAttackAction(weaponIndex));
			case BulletType.Homing:
				return new HomingAttackAction(weaponIndex);
			case BulletType.Direct:
				return new DirectAttackAction(weaponIndex);
			case BulletType.AreaOfEffect:
				return new ExplosionAttackAction(weaponIndex);
			default:
				throw new System.NotSupportedException();
			}			
		}		
	}
}
