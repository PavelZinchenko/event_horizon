using Combat.Collision;

namespace Combat.Ai
{
	public class VanishAction : IAction
	{
		public void Perform(Context context, ref ShipControls controls)
		{
			context.Ship.Vanish();
		}
	}
}
