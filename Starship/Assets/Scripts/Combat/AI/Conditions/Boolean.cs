namespace Combat.Ai.Condition
{
	public class IsFlagSet : ICondition
	{
		public IsFlagSet(State<bool> state)
		{
			_state = state;
		}
			
		public bool IsTrue(Context context) { return _state.Value; }
			
		State<bool> _state;
	}

	public class IsFlagNotSet : ICondition
	{
		public IsFlagNotSet(State<bool> state)
		{
			_state = state;
		}
			
		public bool IsTrue(Context context) { return !_state.Value; }
			
		State<bool> _state;
	}

	public class AlwaysTrue : ICondition
	{
		public bool IsTrue(Context context) { return true; }
	}
}
