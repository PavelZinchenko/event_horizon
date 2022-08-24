namespace Combat.Ai
{
	public class WaitAction : IAction
	{
		public void Perform(Context context, ref ShipControls controls)
		{
			controls.Course = RotationHelpers.Angle(context.Ship.Body.Position.Direction(context.Enemy.Body.Position));
		}
	}
}
