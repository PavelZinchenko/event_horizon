namespace Combat.Ai
{
	public class RecoilAction : IAction
	{
		public RecoilAction(int weaponId)
		{
			_weaponId = weaponId;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			controls.ActivateSystem(_weaponId, true);
			controls.Thrust = 0;
		}
		
		private readonly int _weaponId;
	}
}
