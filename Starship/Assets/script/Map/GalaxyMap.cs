using UnityEngine;
using System.Collections.Generic;
using Galaxy;
using GameServices.Player;
using GameStateMachine.States;
using Services.Messenger;
using Services.ObjectPool;
using Session;
using Zenject;

public class GalaxyMap : MonoBehaviour
{
    [Inject] private readonly StartTravelSignal.Trigger _startTravelTrigger;
    [Inject] private readonly IMessenger _messenger;
    [Inject] private readonly IObjectPool _objectPool;
    [Inject] private readonly ISessionData _session;
    [Inject] private readonly StarMap _starMap;
    [Inject] private readonly StarData _starData;
    [Inject] private readonly MotherShip _motherShip;
    [Inject] private readonly PlayerResources _playerResources;
    [Inject] private readonly PlayerSkills _playerSkills;

    public float Scale = 5;
	public Vector2 ScreenCenter = Vector2.zero;
	public ShipRange Boundary;
	public GameObject StarPrefab;
	public ViewModel.HomeStarPanelViewModel HomeStarPanel;
	public ViewModel.FlightConfirmationViewModel ConfirmationDialog;
	public StarSystem.StarSystem StarSystem;
	public PlayerShipObject PlayerShipObject;

	//public GalaxyMap()
	//{
	//	_self = new WeakReference<GalaxyMap>(this);
	//}

	//public static GalaxyMap Instance
	//{
	//	get { return _self != null ? _self.Target : null; }
	//}

	public void Refresh()
	{
		var camera = Camera.main;
		_center = new Vector2(ScreenCenter.x*camera.aspect, ScreenCenter.y);
		gameObject.Move(_center*camera.orthographicSize - FocusedPosition*Scale);
		UpdateVisibleStars();
	}

	public void OnClick(Vector2 position)
	{
		if (_mapState == ViewMode.StarSystem)
		{
			StarSystem.OnClick(position);
			return;
		}

		//TODO: if (_gameLogic.CurrentPlayerState != PlayerState.Idle)
		//	return;

		var currentStarId = _motherShip.Position;
		var radius = _mapState == ViewMode.GalaxyMap ? 1.6f*Scale : 0.4f*Scale;

		foreach (Transform child in transform)
		{
			if (Vector2.Distance(child.position, position) < radius && child.tag == "Star")
			{
				var starId = System.Convert.ToInt32(child.name);
				if (starId == currentStarId)
					continue;
				
				if (!_motherShip.IsStarReachable(starId))
					Boundary.Refresh();
				else
                    _startTravelTrigger.Fire(starId);

                break;
			}
		}
	}

	public void ClearDestination()
	{
		FocusedPosition = _motherShip.CurrentStar.Position;
		_centerOnPlayerDelay = 0;
	}

	public void OnZoom(float scale)
	{
		Zoom *= scale;
		if (_mapState == ViewMode.StarSystem)
		{
			Zoom = Mathf.Clamp(Zoom, 1f, 2.5f);
		}
		else if (_mapState == ViewMode.StarMap)
		{
			if (Zoom > 25f && scale > 1.02f)
				_motherShip.ViewMode = ViewMode.GalaxyMap;
			else
				Zoom = Mathf.Clamp(Zoom, 5f, 25f);
		}
		else if (_mapState == ViewMode.GalaxyMap)
		{
			if (Zoom < 100f && scale < 0.98f)
				_motherShip.ViewMode = ViewMode.StarMap;
			else
				Zoom = Mathf.Clamp(Zoom, 100f, 150f);
		}
	}

	public void OnMove(Vector2 offset)
	{
		gameObject.Move((Vector2)transform.localPosition + offset);
		_centerOnPlayerDelay = 2;

		UpdateVisibleStars();
	}

	//public void FocusOnStar(int starId)
	//{
	//	if (starId == _playerShip.Position)
	//		return;

	//    FocusedPosition = _starData.GetPosition(starId);
	//	_centerOnPlayerDelay = 0;
	//	ConfirmationDialog.Open(starId);
	//}

    public void OnStarContentChanged(int starId)
    {
        if (starId == _motherShip.CurrentStar.Id)
            UpdateCurrentStar();
        else
            UpdateStar(starId, false);
    }

	public void UpdateCurrentStar()
	{
		var star = _motherShip.CurrentStar;
		UpdateStar(star.Id);
		if (StarSystem.IsActive)
		{
			StarSystem.Cleanup();
			StarSystem.Initialize(star, Star.GetStarColor(star.Id));
		}
	}

    private void UpdateZoom()
	{
		var camera = Camera.main;
		var current = camera.orthographicSize;
		if (Mathf.Abs(current - Zoom) < 0.005f)
			return;

		var size = Mathf.Lerp(current, Zoom, 5*UpdateCooldown);
		var scale = size / current;
        gameObject.Move((Vector2)transform.localPosition + _center*current * (scale - 1f));
        camera.orthographicSize = size;
		UpdateVisibleStars();
	}

	private void Awake()
	{
		PlayerShipObject.MovedEvent += OnShipMoved;
		StarSystem.MovedEvent += OnShipMoved;
        Boundary.transform.localScale = Vector3.one * Scale * _playerSkills.MainFilghtRange;
    }

	private void OnDisable()
	{
        _session.StarMap.MapScaleFactor = _mapZoom;
		_session.StarMap.StarScaleFactor = _starZoom;
	}

	private void Start()
	{
		var camera = Camera.main;
        _mapZoom = Mathf.Clamp(_session.StarMap.MapScaleFactor, 5f, 25f);
		_starZoom = Mathf.Clamp(_session.StarMap.StarScaleFactor, 1f, 2.5f);
		_galaxyZoom = 100f;

        _messenger.AddListener<int>(EventType.PlayerPositionChanged, OnPlayerPositionChanged);
        _messenger.AddListener<int>(EventType.FocusedPositionChanged, OnFocusedPositionChanged);

        _messenger.AddListener<ViewMode>(EventType.ViewModeChanged, OnMapStateChanged);
        _messenger.AddListener<int>(EventType.StarContentChanged, OnStarContentChanged);
        _messenger.AddListener(EventType.StarMapChanged, OnStarMapChanged);

		OnPlayerPositionChanged(_motherShip.Position);
		OnMapStateChanged(_motherShip.ViewMode);

        camera.orthographicSize = Zoom;
		Refresh();
	}

	private void Update()
	{
	    _timeLeft -= Time.deltaTime;
	    if (_timeLeft > 0) return;
	    _timeLeft = UpdateCooldown;

        if (_centerOnPlayerDelay > 0)
		{
			_centerOnPlayerDelay -= UpdateCooldown;
		}
		else
		{
			var position = _center*Camera.main.orthographicSize -  FocusedPosition*Scale;

		    if (_mapState == ViewMode.StarSystem)
		    {
		        gameObject.Move(Vector2.Lerp(transform.localPosition, position, 5*UpdateCooldown));
		    }
            else if (Vector2.Distance(transform.localPosition, position) > 0.01f)
			{
				gameObject.Move(Vector2.Lerp(transform.localPosition, position, 5*UpdateCooldown));
				UpdateVisibleStars();
			}
		}

		UpdateZoom();
		Boundary.gameObject.Move(_motherShip.CurrentStar.Position*Scale);
	}

	private void OnMapStateChanged(ViewMode state)
	{
		if (_mapState == state)
			return;

		if (state == ViewMode.StarSystem)
		{
			var star = _motherShip.CurrentStar;
			var color = Star.GetStarColor(star.Id);

			StarSystem.gameObject.Move(star.Position*Scale);
			StarSystem.Initialize(star, color);
		}
		else if (_mapState == ViewMode.StarSystem)
		{
			StarSystem.Cleanup();
			FocusedPosition = _motherShip.CurrentStar.Position;
		}

        PlayerShipObject.gameObject.SetActive(state == ViewMode.StarMap || state == ViewMode.GalaxyMap);

		_mapState = state;		
		_centerOnPlayerDelay = 0f;
	}

    private void OnFocusedPositionChanged(int starId)
    {
        if (starId >= 0)
        {
            FocusedPosition = _starData.GetPosition(starId);
            _centerOnPlayerDelay = 0;
        }
        else
        {
            ClearDestination();
        }
    }

	private Vector2 FocusedPosition { get; set; }
	
	private void UpdateStar(int starId, bool createIfNotActive = true)
	{
		var currentStar = new Galaxy.Star(starId, _starData);
		
		Star star;
		if (_stars.TryGetValue(starId, out star))
		{
			star.Deinitialize();
			star.Initialize(currentStar);
		}
		else if (createIfNotActive)
		{
			_stars[starId] = CreateStar(currentStar);
		}
	}	

	private void OnPlayerPositionChanged(int starId)
	{
		UpdateStar(starId);
		FocusedPosition = _starData.GetPosition(starId);
	}

	private void OnShipMoved(Vector2 position)
	{
		Boundary.Hide();
		FocusedPosition = position/Scale;
    }

    private void OnStarMapChanged()
    {
        UpdateVisibleStars(true);
    }

	private void UpdateVisibleStars(bool forceUpdate = false)
	{
		var camera = Camera.main;
		var screenSize = new Vector2(camera.orthographicSize*camera.aspect, camera.orthographicSize);
		var topLeft = -(Vector2)transform.localPosition - screenSize;
		var bottomRight = -(Vector2)transform.localPosition + screenSize;

		var temp = _starsOld;
		_starsOld = _stars;
		_stars = temp;
		_stars.Clear();

        var allStars = camera.orthographicSize > 30f ? _starMap.GetGalaxyViewVisibleStars(topLeft/Scale, bottomRight/Scale)
			: _starMap.GetVisibleStars(topLeft/Scale, bottomRight/Scale);

		foreach (var item in allStars)
		{
			Star star;
			if (_starsOld.TryGetValue(item.Id, out star))
			{
				_starsOld.Remove(item.Id);

				if (forceUpdate)
				{
					star.Deinitialize();
					star.Initialize(item);
				}
				
				_stars.Add(item.Id, star);
			}
			else
			{
				_stars.Add(item.Id, CreateStar(item));
			}
		};

		foreach (var item in _starsOld)
			DestroyStar(item.Value.GetComponent<Star>());

		var position = _starData.GetPosition(0)*Scale;
		var homeStarVisible = topLeft.x > position.x != bottomRight.x > position.x && topLeft.y > position.y != bottomRight.y > position.y;
		var direction = _center*Camera.main.orthographicSize - position - (Vector2)transform.localPosition;
		HomeStarPanel.SetDistance(direction/Scale);
		HomeStarPanel.Visible = !homeStarVisible && _mapState != ViewMode.StarSystem;
	}

	private Star CreateStar(Galaxy.Star star)
	{
		var starGameObject = _objectPool.GetObject(StarPrefab);
	    var position = star.Position;
		starGameObject.SetActive(true);
		starGameObject.transform.parent = transform;
		starGameObject.transform.localPosition = new Vector3(position.x*Scale,position.y*Scale,-1);
		var script = starGameObject.GetComponent<Star>();

		script.Initialize(star);

		return script;
	}

	private void DestroyStar(Star star)
	{
		star.Deinitialize();
		_objectPool.ReleaseObject(star.gameObject);
	}

	private float Zoom
	{
		get
		{
			if (_mapState == ViewMode.StarMap)
				return _mapZoom;
			if (_mapState == ViewMode.StarSystem)
				return _starZoom;

			return _galaxyZoom;
		}
		set
		{
			if (_mapState == ViewMode.StarMap)
				_mapZoom = value;
			else if (_mapState == ViewMode.StarSystem)
				_starZoom = value; 
			else
				_galaxyZoom = value;
		}
	}

    private float _timeLeft;
	private float _mapZoom;
	private float _starZoom;
	private float _galaxyZoom;
	private ViewMode _mapState;
	private Vector2 _center;
	private float _centerOnPlayerDelay = 0;
	private Dictionary<int, Star> _stars = new Dictionary<int, Star>();
	private Dictionary<int, Star> _starsOld = new Dictionary<int, Star>();
	//private static WeakReference<GalaxyMap> _self;
    private const float UpdateCooldown = 0.01f;
}
