namespace Combat.Ai.Condition
{
	public class All : ICondition
	{
		public All(params ICondition[] conditions)
		{
			_conditions = conditions;
		}
			
		public bool IsTrue(Context context)
		{
			foreach (var item in _conditions)
				if (!item.IsTrue(context))
					return false;
			return true;
		}
			
		private readonly ICondition[] _conditions;
	}
		
	public class Any : ICondition
	{
		public Any(params ICondition[] conditions)
		{
			_conditions = conditions;
		}
			
		public bool IsTrue(Context context)
		{
			foreach (var item in _conditions)
				if (item.IsTrue(context))
					return true;
			return false;
		}
			
		private readonly ICondition[] _conditions;
	}

	public class AnyNot : ICondition
	{
		public AnyNot(params ICondition[] conditions)
		{
			_conditions = conditions;
		}
			
		public bool IsTrue(Context context)
		{
			foreach (var item in _conditions)
				if (!item.IsTrue(context))
					return true;
			return false;
		}
			
		private readonly ICondition[] _conditions;
	}

	public class None : ICondition
	{
		public None(params ICondition[] conditions)
		{
			_conditions = conditions;
		}
			
		public bool IsTrue(Context context)
		{
			foreach (var item in _conditions)
				if (item.IsTrue(context))
					return false;
			return true;
		}
			
		private readonly ICondition[] _conditions;
	}
}
