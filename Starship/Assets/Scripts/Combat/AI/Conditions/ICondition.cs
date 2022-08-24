using Combat.Component.Unit.Classification;
using Combat.Unit;
using UnityEngine;

namespace Combat.Ai
{
	public class State<T> where T: struct
	{
        public State(T value = default(T))
        {
            Value = value;
        }

		public T Value { get; set; }
	}

	public interface ICondition
	{
		bool IsTrue(Context context);
	}

    public class AlwaysTrueCondition : ICondition
	{
		public bool IsTrue(Context context) { return true; }
	}

	public class IsTrueCondition : ICondition
	{
		public IsTrueCondition(State<bool> state)
		{
			_state = state;
		}

		public bool IsTrue(Context context) { return _state.Value; }

		State<bool> _state;
	}

	public class IsFalseCondition : ICondition
	{
		public IsFalseCondition(State<bool> state)
		{
			_state = state;
		}
		
		public bool IsTrue(Context context) { return !_state.Value; }
		
		State<bool> _state;
	}
	
	public class HasThreatsCondition : ICondition
	{
		public HasThreatsCondition(float timeMin)
		{
			_timeMin = timeMin;
		}

		public bool IsTrue(Context context)
		{
			return context.Threats != null && context.Threats.TimeToHit <= _timeMin;
		}

		private float _timeMin;
	}
	
	public class AwakeCondition : ICondition
	{
		public AwakeCondition(int wakeTimePecentage)
		{
			_threshold = Mathf.Clamp01(wakeTimePecentage / 100f)*5f;
		}
		
		public bool IsTrue(Context context)
		{
			return Mathf.Repeat(context.CurrentTime, 5f) <= _threshold;
        }
        
		private float _threshold;
    }
	
    public class EnergyRechargedCondition : ICondition
	{
		public EnergyRechargedCondition(float min, float max, State<bool> isRecharging)
		{
			_min = min;
			_max = max;
			_isRecharging = isRecharging;
		}

		public bool IsTrue(Context context)
		{
			var ship = context.Ship;
			if (ship.Stats.Energy.Percentage <= _min)
				_isRecharging.Value = true;
			else if (ship.Stats.Energy.Percentage >= _max)
				_isRecharging.Value = false;

			return !_isRecharging.Value;
		}

		private readonly float _min;
		private readonly float _max;
		private readonly State<bool> _isRecharging;
    }

	public class DistanceCondition : ICondition
	{
		public DistanceCondition(float min, float max)
		{
			_min = min;
			_max = max;
		}
		
		public bool IsTrue(Context context)
		{
			var distance = Helpers.Distance(context.Ship, context.Enemy);
			return distance >= _min && distance < _max;
		}
		
		private readonly float _min;
		private readonly float _max;
	}

	public class HasEnergyCondition : ICondition
	{
		public HasEnergyCondition(float value)
		{
			_min = value;
		}
		
		public bool IsTrue(Context context)
		{
			return context.UnusedEnergy.Value / context.Ship.Stats.Energy.MaxValue > _min;
		}
		
		private readonly float _min;
	}	

	public class HitPointsCondition : ICondition
	{
		public HitPointsCondition(float min, float max)
		{
			_min = min;
			_max = max;
		}
		
		public bool IsTrue(Context context)
		{
			var hitPoints = context.Ship.Stats.Armor.Percentage;
			return hitPoints >= _min && hitPoints <= _max;
		}
		
		private readonly float _min;
		private readonly float _max;
	}	

	public class IsEnemyContition : ICondition
	{
		public bool IsTrue(Context context)
		{
			return context.Ship.Type.Side.IsEnemy(context.Enemy.Type.Side);
		}
	}

	public class NotCondition : ICondition
	{
		public NotCondition(ICondition condition)
		{
			_condition = condition;
		}

		public bool IsTrue(Context context)
		{
			return !_condition.IsTrue(context);
		}

		private readonly ICondition _condition;
	}

	public class DroneOutOfRangeCondition : ICondition
	{
		public DroneOutOfRangeCondition(float range)
		{
			_range = range;
		}
		
		public bool IsTrue(Context context)
		{
            var parent = context.Ship.Type.Owner;
            if (parent.IsActive())
                return Helpers.Distance(context.Ship, context.Ship.Type.Owner) > _range;
            else
                return false;
		}
		
		private readonly float _range;
	}

	public class DroneTargetOutOfRangeCondition : ICondition
	{
		public DroneTargetOutOfRangeCondition(float range)
		{
			_range = range;
		}
		
		public bool IsTrue(Context context)
		{
            var parent = context.Ship.Type.Owner;
            if (parent.IsActive())
                return Helpers.Distance(context.Enemy, parent) > _range;
            else
                return false;
		}
		
		private readonly float _range;
	}

	public class MothershipRetreatedCondition : ICondition
	{
		public bool IsTrue(Context context)
		{
			var parent = context.Ship.Type.Owner;
			return parent != null && parent.State == UnitState.Inactive;
		}
	}
}
