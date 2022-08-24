using Combat.Unit;

namespace Combat.Ai
{
	public class DroneReturnAction : IAction
	{
		public DroneReturnAction(float distance)
		{
			_action = new KeepDistanceAction(distance*0.99f, distance*1.01f, 1);
		}

		public void Perform(Context context, ref ShipControls controls)
		{
            var parent = context.Ship.Type.Owner;
            if (!parent.IsActive())
                return;

            context.Enemy = parent;
            _action.Perform(context, ref controls);
        }
		
		private readonly KeepDistanceAction _action;
	}
}
