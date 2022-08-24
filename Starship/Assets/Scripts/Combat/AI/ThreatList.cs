using System.Collections.Generic;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Scene;
using Combat.Unit;
using UnityEngine;

namespace Combat.Ai
{
	public class ThreatList
	{
	    public ThreatList(IScene scene)
	    {
	        _scene = scene;
	    }

		public System.Collections.ObjectModel.ReadOnlyCollection<IUnit> Units { get { return _threats.AsReadOnly(); } }
		public float TimeToHit { get { return _timeToHit; } }

		public void Update(float elapsedTime, IShip ship, IStrategy strategy)
		{
			_cooldown -= elapsedTime;
			if (_cooldown > 0)
				return;

			_threats.Clear();
			_timeToHit = 1000f;

			if (!ship.IsActive() || strategy == null)
				return;

			_cooldown = UpdateInterval;
			
			_scene.Units.GetObjectsInRange(_threats, ship.Body.Position, 20);
			
			_timeToHit = 1000f;
			var index = 0;
			for (var i = 0; i < _threats.Count; ++i)
			{
			    var item = _threats[i];

			    switch (item.Type.Class)
			    {
                    case UnitClass.Ship:
                    case UnitClass.Drone:
                    case UnitClass.Decoy:
                    case UnitClass.Loot:
                    case UnitClass.Camera:
                        continue;
			    }

				if (item.Type.Side.IsAlly(ship.Type.Side))
					continue;
				
				if (!strategy.IsThreat(ship, item))
					continue;

				//if (collider is CombatSystem.Collision.IVectorCollider)
				//{
				//	var vector = (CombatSystem.Collision.IVectorCollider)collider;
				//	var distance = Geometry.Point2VectorDistance(vector.StartPoint.Direction(ship.Position), vector.Direction);
				//	if (distance < ship.Size/2)
				//	{
				//		_threats[index++] = item;
				//		_timeToHit = 0;
				//	}
				//}
				//else
				{
					var itemPosition = ship.Body.Position.Direction(item.Body.Position);
					var velocity = ship.Body.Velocity - item.Body.Velocity;
					var dir = velocity.normalized;
					var len = Mathf.Max(0, Vector2.Dot(itemPosition, dir));
					
					var distance = Vector2.Distance(len*dir, itemPosition);
					var threatTime = len/velocity.magnitude;
					if (2*distance <= item.Body.Scale + ship.Body.Scale && threatTime < 5f)
					{
						_threats[index++] = item;
						
						if (threatTime < _timeToHit)
							_timeToHit = threatTime;
					}
				}
			}

			var count = _threats.Count - index;
			if (count > 0)
				_threats.RemoveRange(index, count);
		}

		private float _timeToHit = 1000f;
		private float _cooldown;
		private const float UpdateInterval = 0.1f;
	    private readonly IScene _scene;
        private readonly List<IUnit> _threats = new List<IUnit>();
    }
}
