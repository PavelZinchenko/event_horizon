using UnityEngine;
using Combat.Component.Ship;

namespace Combat.Ai
{
	public class ChargedWeaponAction : IAction
	{
		public ChargedWeaponAction(int weaponId)
		{
			_weaponId = weaponId;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			var ship = context.Ship;
			var weapon = ship.Systems.All.Weapon(_weaponId);

		    if (weapon.Active || context.UnusedEnergy.Value >= weapon.Info.EnergyCost || Mathf.Approximately(context.UnusedEnergy.Value, ship.Stats.Energy.MaxValue))
		    {
		        context.UnusedEnergy.Value -= weapon.Info.EnergyCost;
		        controls.ActivateSystem(_weaponId, true);
		    }
		}
		
		private readonly int _weaponId;
	}
}
