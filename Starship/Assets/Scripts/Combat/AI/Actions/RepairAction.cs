namespace Combat.Ai
{
	public class RepairAction : IAction
	{
		public RepairAction(int deviceId)
		{
			_deviceId = deviceId;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
		    if (context.Threats == null) return;

			if (context.Threats.TimeToHit >= 5 && context.Ship.Stats.Armor.Percentage < 1f)
				controls.ActivateSystem(_deviceId);
		}
		
		private readonly float _distance;
		private readonly int _deviceId;
	}
}
