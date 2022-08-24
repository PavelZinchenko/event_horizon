namespace Combat.Ai.Condition
{
	public class HasEnergy : ICondition
	{
		public HasEnergy(float value)
		{
			_min = value;
		}
			
		public bool IsTrue(Context context)
		{
			if (context.Ship.Stats.Energy.MaxValue <= 0)
			    return false;
			return context.UnusedEnergy.Value / context.Ship.Stats.Energy.MaxValue > _min;
		}
			
		private readonly float _min;
	}

	public class EnemyHasEnergy : ICondition
	{
		public EnemyHasEnergy(float value)
		{
			_min = value;
		}
			
		public bool IsTrue(Context context)
		{
			return context.Enemy.Stats.Energy.Percentage > _min;
		}
			
		private readonly float _min;
	}
}
