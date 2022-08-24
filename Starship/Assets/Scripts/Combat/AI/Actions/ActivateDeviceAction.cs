namespace Combat.Ai
{
	public class ActivateDeviceAction : IAction
	{
		public ActivateDeviceAction(int deviceId, State<float> timerState = null)
		{
			_deviceId = deviceId;
		    _timerState = timerState;
		}
		
		public void Perform(Context context, ref ShipControls controls)
		{
			controls.ActivateSystem(_deviceId);
		    if (_timerState != null)
		        _timerState.Value = context.CurrentTime;
		}
		
		private readonly int _deviceId;
	    private readonly State<float> _timerState;
	}
}
