using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Unit;
using UnityEngine;

namespace Combat.Ai
{
	public class ControlledWeaponAction : IAction
	{
		public ControlledWeaponAction(int weaponId, bool secondaryTargets)
		{
			_weaponId = weaponId;
		    _secondaryTargets = secondaryTargets;
		}

        public void Perform(Context context, ref ShipControls controls)
        {
            var ship = context.Ship;
            var enemy = context.Enemy;

            if (_secondaryTargets && context.Targets != null)
            {
                var targets = context.Targets.Items;
                var shouldActivate = false;
                for (var i = 0; i < targets.Count; i++)
                    shouldActivate |= ShouldActivate(ship, targets[i]);
                controls.ActivateSystem(_weaponId, shouldActivate);
            }
            else
            {
                controls.ActivateSystem(_weaponId, ShouldActivate(ship, enemy));
            }
        }

        public bool ShouldActivate(IShip ship, IShip enemy)
		{
            var weapon = ship.Systems.All.Weapon(_weaponId);
            if (!weapon.Active)
                return false;

            var bullet = weapon.ActiveBullet;
            if (!bullet.IsActive())
                return false;

            var dir = bullet.Body.WorldPosition().Direction(enemy.Body.Position).normalized;
            var delta = Vector2.Dot(bullet.Body.WorldVelocity(), dir) - Vector2.Dot(enemy.Body.WorldVelocity(), dir);

		    return delta >= 0;
        }
		
		private readonly int _weaponId;
	    private readonly bool _secondaryTargets;

	}
}
