using GameDatabase.DataModel;
using UnityEngine;
using Services.Reources;

namespace StarSystem
{
	public class Ship : MonoBehaviour
	{
		public SpriteRenderer Sprite;

		public float EnginePower { get { return _enginePower; } set { _enginePower = value; } }
		
		public void Initialize(GameDatabase.DataModel.Ship ship, IResourceLocator resourceLocator)
		{
			CreateShip(ship, resourceLocator);
			Update();
		}

		public void InitializeFlagship(GameDatabase.DataModel.Ship ship, IResourceLocator resourceLocator)
		{
			CreateShip(ship, resourceLocator);
			_immovable = true;
		}
        
		public void MoveTo(Vector2 position)
		{
			_isMoving = true;
			_start = ShipPosition + (Vector2)transform.localPosition - position;
			var distance = _start.magnitude;
			_timeScale = distance/_shipSpeed;
            
            ShipPosition = _start;

			_control = _start + 0.5f*distance*RotationHelpers.Direction(Rotation);
			gameObject.Move(position);
			_progress = 0f;
			_immovable = false;
		}

		public Vector2 Position { get { return (Vector2)transform.localPosition + ShipPosition; } }

		public float Rotation
		{
			get { return Sprite.transform.localEulerAngles.z; }
			set { Sprite.transform.localEulerAngles = new Vector3(0,0,value); }
		}

		private void CreateShip(GameDatabase.DataModel.Ship ship, IResourceLocator resourceLocator)
		{
			var size = 0.5f + ship.ModelScale/5f;
			var random = new System.Random(GetHashCode());

            Sprite.sprite = resourceLocator.GetSprite(ship.ModelImage);
			Sprite.transform.parent = transform;
			Sprite.transform.localPosition = Vector3.zero;
			Sprite.transform.localEulerAngles = Vector3.zero;
			Sprite.transform.localScale = size*Vector3.one;
			
			_shipSpeed = 5f*(0.8f + 0.4f*random.NextFloat())/(size+0.5f);
			_shipOrbitRadius = 2f*(random.NextFloat()*0.4f + 0.8f + size*0.1f);
			_angularVelocity = (random.Next()%2 == 0 ? _shipSpeed : -_shipSpeed)/(2*Mathf.PI*_shipOrbitRadius);
			_shipOrbitalAngle = random.Next(360);
			_isMoving = false;
		}

		private Vector2 ShipPosition
		{
			get { return Sprite.transform.localPosition; }
			set { Sprite.gameObject.Move(value); }
        }
        
		private void Update()
		{
			var delta = Time.deltaTime;

            if (_isMoving && _progress < 1f)
			{
				_shipOrbitalAngle = _angularVelocity > 0 ? Rotation - 90 : Rotation + 90;
				var end = _shipOrbitRadius*RotationHelpers.Direction(_shipOrbitalAngle);

				ShipPosition = Geometry.Bezier(_start, _control, end, _progress);
				var dir = Geometry.BezierTangent(_start, _control, end, _progress);

				if (dir.sqrMagnitude > float.Epsilon)
                    Rotation = RotationHelpers.Angle(dir);

				_progress += EnginePower*delta/_timeScale;
            }
			else if (!_immovable)
			{
				_isMoving = false;
				_shipOrbitalAngle += _angularVelocity*delta*100;
				ShipPosition = _shipOrbitRadius*RotationHelpers.Direction(_shipOrbitalAngle);
				Rotation = _shipOrbitalAngle + (_angularVelocity > 0 ? 90f : -90f);
			}
		}

		private bool _immovable;
		private bool _isMoving;
		private float _angularVelocity;
		private float _shipOrbitalAngle;
		private float _shipSpeed;
		private float _shipOrbitRadius;
		private Vector2 _start;
		private Vector2 _control;
		private float _timeScale;
		private float _progress;
		private float _enginePower = 1.0f;
	}
}
