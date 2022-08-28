using System.Collections.Generic;
using Combat.Component.Ship;
using Combat.Component.Unit;

namespace Combat.Ai
{
	public abstract class StrategyBase : IStrategy
	{
		public abstract bool IsThreat(IShip ship, IUnit unit);

		public void Apply(Context context)
		{
			var controls = new ShipControls(context.Ship);
			foreach (var policy in _policyList)
			    policy.Perform(context, ref controls);

            controls.Apply(context.Ship);
		}

		public void AddPolicy(ICondition condition, IAction action, IAction oppositeAction = null)
		{
			_policyList.Add(new Policy(condition, action, oppositeAction));
		}

		private readonly List<Policy> _policyList = new List<Policy>();
	}
}
