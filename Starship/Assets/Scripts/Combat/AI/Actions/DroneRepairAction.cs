using Combat.Unit;

namespace Combat.Ai
{
	public class DroneRepairAction : DirectAttackAction
	{
		public DroneRepairAction(int weaponId)
			: base(weaponId)
		{
		}
		
		public override void Perform(Context context, ref ShipControls controls)
		{
            var parent = context.Ship.Type.Owner;
            if (!parent.IsActive() || parent.Stats.Armor.Percentage >= 0.99f || parent.Stats.TimeFromLastHit < 1.0f)
                return;

            context.Enemy = parent;
            base.Perform(context, ref controls);
        }
	}
}
