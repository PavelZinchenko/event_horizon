using Galaxy;
using GameObjects;
using GameServices.Player;
using Services.Audio;
using Services.Messenger;
using UnityEngine;
using Zenject;

public class PlayerShipObject : MonoBehaviour
{
    [Inject] private readonly ISoundPlayer _soundPlayer;
    [Inject] private readonly MotherShip _motherShip;
    [Inject] private readonly StarData _starData;

    public Texture2D Texture;
    public Color Color = Color.white;
    public float ShipSize = 1.5f;
    public float ShipOrbitRadius = 1.0f;
    public float Scale = 7.0f;
    public AudioClip EngineSound;

    [Inject]
    private void Initialize(IMessenger messenger)
    {
        messenger.AddListener<int, int, float>(EventType.PlayerShipMoved, OnShipMoved);
        messenger.AddListener<int>(EventType.PlayerPositionChanged, OnPositionChanged);
        messenger.AddListener<ViewMode>(EventType.ViewModeChanged, OnMapStateChanged);

        gameObject.Move(_motherShip.CurrentStar.Position * Scale);
        CreateShip();
    }

	public bool IsPlaying 
	{
		get
		{
			return _isPlaying;
		}
		set 
		{
			if (value == _isPlaying)
				return;

			if (value)
				_soundPlayer.Play(EngineSound, GetHashCode(), true);
			else
				_soundPlayer.Stop(GetHashCode());

            _isPlaying = value;
		}
	}

	public event System.Action<Vector2> MovedEvent = position => {};

	private void OnPositionChanged(int starId)
	{
		_startId = _endId = -1;
		IsPlaying = _isMoving = false;

		gameObject.Move(_starData.GetPosition(starId) * Scale);
	}

	private void OnShipMoved(int source, int target, float progress)
	{
		IsPlaying = _isMoving = true;

		if (_startId != source || _endId != target)
		{
			_startId = source;
			_endId = target;

			var targetPosition = _starData.GetPosition(_endId)*Scale;

			_start = _ship.Position + ((Vector2)transform.localPosition - targetPosition);
			_end = ShipOrbitRadius*_start.normalized;
			_shipOrbitalAngle = Mathf.Atan2(_start.x, _start.y);
			_control = _start + 0.5f*Mathf.Min(Vector2.Distance(_end,_start), Scale)*RotationHelpers.Direction(_ship.Rotation);

			gameObject.Move(targetPosition);
			_ship.Position = _start;
		}

		var current = Geometry.Bezier(_start, _control, _end, progress);
		var dir = current - _ship.Position;
		_ship.Position = current;
		if (dir.sqrMagnitude > float.Epsilon)
			_ship.Rotation = RotationHelpers.Angle(dir);

		MovedEvent((Vector2)transform.localPosition + current);
	}

	private void Update()
	{
		if (_isMoving || _galaxyMapMode || _ship == null)
			return;

		var delta = Time.deltaTime;
		_shipOrbitalAngle += delta/20;
		_ship.Position = new Vector2(ShipOrbitRadius*Mathf.Sin(_shipOrbitalAngle), ShipOrbitRadius*Mathf.Cos(_shipOrbitalAngle));
		_ship.Rotation = _ship.Rotation + Mathf.Rad2Deg*delta/12;
	}

	private void CreateShip()
	{
		var position = Vector3.zero;
		var rotation = 0f;

		_ship = Ship.Create(Texture);
		_ship.Color = Color;
		_ship.gameObject.transform.parent = transform;
		_ship.gameObject.transform.localPosition = position;
		_ship.Rotation = rotation;
		_ship.Scale = ShipSize;
		_ship.gameObject.SetActive(true);

		OnMapStateChanged(_motherShip.ViewMode);
	}

	private void OnMapStateChanged(ViewMode state)
	{
		_galaxyMapMode = state == ViewMode.GalaxyMap;

		if (_galaxyMapMode)
		{
			_ship.Position = Vector2.zero;
			_ship.Rotation = 90;
			_ship.Scale = 6*ShipSize;
		}
		else
		{
			_ship.Scale = ShipSize;
		}
	}

	private bool _isMoving;
	private int _startId;
	private int _endId;

	private bool _galaxyMapMode = false;
	private Ship _ship;
	private Vector2 _start;
	private Vector2 _control;
	private Vector2 _end;
	private float _shipOrbitalAngle;
	private bool _isPlaying;
}
