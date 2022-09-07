using System;
using UnityEngine;
using System.Collections.Generic;
using Galaxy;
using GameServices.Player;
using Services.Localization;
using Services.ObjectPool;
using Zenject;
using Random = UnityEngine.Random;

public class Star : MonoBehaviour
{
	public GameObject HaloObject;
	public GameObject StarObject;
	public GameObject MiniObject;
	public float Size = 1.0f;
	public GameObject StarNamePrefab;
    public GameObject HomeIconPrefab;
	public GameObject QuestIconPrefab;
	public GameObject DangerIconPrefab;
	public GameObject FactionIconPrefab;
	public GameObject WormholeIconPrefab;
	public GameObject MultiplayerIconPrefab;
	public GameObject ArenaIconPrefab;
	public GameObject LabIconPrefab;
	public GameObject RuinsIconPrefab;
    public GameObject XmasIconPrefab;
	public GameObject BossIconPrefab;
	public GameObject MilitaryIconPrefab;
	public GameObject ChallengeIconPrefab;
	public GameObject StarBorderPrefab;
	public GameObject GuardianIconPrefab;
	public GameObject PassiveGuardianIconPrefab;
	public GameObject BlackMarketIconPrefab;
    public GameObject QuestObjective;
	public GameObject StarInfo;
    public GameObject MiniBossIcon;
    public GameObject MiniShopIcon;
    public GameObject MiniArenaIcon;
    public GameObject MiniQuestObjective;
    public GameObject MiniXmasIcon;
    public GameObject PandemicIcon;
    public Color UnknownStarColor;

	public enum State { Galaxy, Map, Normal, StarSystem, Hidden }

	public float SwitchToGalaxyDistance = 100f;
	public float SwitchToMapDistance = 20f;
	public float SwitchToStarSystemDistance = 4.5f;

    [Inject] private readonly IObjectPool _objectPool;
    [Inject] private readonly ILocalization _localization;
    [Inject] private readonly MotherShip _motherShip;
    [Inject] private readonly StarMap _starMap;

    public void Initialize(Galaxy.Star star)
	{
		_starId = star.Id;
		name = star.Id.ToString();

		Random.InitState(star.Id);
		_scale = Size * (0.5f + Random.value*0.75f);
		_blinkSpeed = 1 + 2*Random.value*Random.value;
		_state = State.Normal;
        _showMiniStarOnGalaxyMap = _starMap.ShouldBeVisibleOnGalaxyMap(star.Id);

        HaloObject.transform.localScale = Vector3.one * _scale;
		HaloObject.transform.localEulerAngles = new Vector3(0,0,Random.Range(0,360));
		HaloObject.GetComponent<Renderer>().material.color = GetStarColor(star.Id);
		StarObject.transform.localScale = Vector3.one * Size * 0.2f;
        _miniObjectScale = Vector3.one*Size*(star.IsVisited ? 0.4f : 0.2f);
        MiniObject.transform.localScale = _miniObjectScale;
		var color = star.Region.Faction.Color;

		MiniObject.GetComponent<Renderer>().material.color = star.IsVisited ? (Color)color : UnknownStarColor;

		if (star.IsVisited)
			AddBorder(star, color);

	    if (star.IsQuestObjective)
	    {
	        AddIcon(QuestObjective);
	        AddIcon(MiniQuestObjective);
	        _showMiniStarOnGalaxyMap = false;
	    }

        if (star.Id == 0)
		{
			AddIcon(HomeIconPrefab);
		}
		else if (star.IsVisited)
		{
			var starname = AddIcon(StarNamePrefab).GetComponent<TextMesh>();
			starname.text = string.IsNullOrEmpty(star.Bookmark) ? star.Name : star.Bookmark;
            starname.color = color;

			if (star.HasStarBase)
			{
				AddIcon(FactionIconPrefab).GetComponent<StarIcon>().SetColor(color);
				AddStarInfo(star);
			    _showMiniStarOnGalaxyMap = false;
			}
            else if (_showMiniStarOnGalaxyMap && star.HasBookmark)
            {
                AddStarBookmark(star);
            }

		    var guardian = star.Occupant;
		    if (guardian.IsExists)
		    {
		        if (guardian.CanBeAggressive)
                    AddIcon(GuardianIconPrefab);
                else
                    AddIcon(PassiveGuardianIconPrefab);
            }

            var objects = star.Objects;

		    if (objects.Contain(StarObjectType.Event) && star.LocalEvent.IsActive)
		    {
		        AddIcon(QuestIconPrefab);
		    }
		    if (objects.Contain(StarObjectType.Survival))
			{
				AddIcon(DangerIconPrefab);
			}
			else if (objects.Contain(StarObjectType.Boss) && !star.Boss.IsDefeated)
			{
				AddIcon(BossIconPrefab);
			    if (_starMap.ShowBosses)
			    {
			        AddIcon(MiniBossIcon).GetComponent<StarIcon>().SetColor(Color.Lerp(color, Color.white, 0.3f));
			        _showMiniStarOnGalaxyMap = false;
			    }
			}
			else if (objects.Contain(StarObjectType.Ruins) && !star.Ruins.IsDefeated)
			{
				AddIcon(RuinsIconPrefab);
			}
            else if (objects.Contain(StarObjectType.Xmas))
            {
                AddIcon(XmasIconPrefab);
                if (_starMap.ShowXmas)
                {
                    AddIcon(MiniXmasIcon);
                    _showMiniStarOnGalaxyMap = false;
                }
            }
            else if (objects.Contain(StarObjectType.Wormhole))
			{
				AddIcon(WormholeIconPrefab);
			}
			else if (objects.Contain(StarObjectType.Arena))
			{
				AddIcon(ArenaIconPrefab);
                if (_starMap.ShowArenas)
                {
                    AddIcon(MiniArenaIcon).GetComponent<StarIcon>().SetColor(Color.Lerp(color, Color.white, 0.3f));
                    _showMiniStarOnGalaxyMap = false;
                }
            }
            else if (objects.Contain(StarObjectType.Challenge) && !star.Challenge.IsCompleted)
			{
				AddIcon(ChallengeIconPrefab);
			}
			//else if (star.HasPointOfInterest(Game.PointOfInterest.Laboratory))
			//{
			//	AddIcon(LabIconPrefab);
			//}
            else if (objects.Contain(StarObjectType.Military))
            {
                AddIcon(MilitaryIconPrefab);
            }
            else if (objects.Contain(StarObjectType.Hive) && !star.Pandemic.IsDefeated)
            {
                AddIcon(PandemicIcon);
            }
			else if (objects.Contain(StarObjectType.BlackMarket))
			{
				AddIcon(BlackMarketIconPrefab);
			    if (_starMap.ShowStores)
			    {
			        AddIcon(MiniShopIcon);
			        _showMiniStarOnGalaxyMap = false;
			    }
			}
		}

		OnStateChanged();
	}

	public void Deinitialize()
	{
		var itemsToDelete = new List<GameObject>();
		foreach (Transform child in transform)
		{
			var item = child.gameObject;
			if (item == HaloObject || item == StarObject || item == MiniObject)
				continue;
			itemsToDelete.Add(item);
		}

		foreach (var item in itemsToDelete)
			_objectPool.ReleaseObject(item);
	}

	void Update()
	{
		var size = _mainCamera.orthographicSize;

	    State state;
		if (size >= SwitchToGalaxyDistance)
			state = State.Galaxy;
		else if (size >= SwitchToMapDistance)
			state = State.Map;
		else if (size >= SwitchToStarSystemDistance)
			state = State.Normal;
		else if (_motherShip.Position == _starId)
			state = State.StarSystem;
		else
			state = State.Hidden;

		if (state != _state)
		{
			_state = state;
			OnStateChanged();
		}

		if (_state == State.Normal)
		{
			HaloObject.transform.localScale = Vector3.one * _scale * (1 + 0.2f*Mathf.Sin(_blinkSpeed*Time.time));
		}
		else if (_state == State.StarSystem)
		{
			var scale = size/SwitchToStarSystemDistance;
			HaloObject.transform.localScale = Vector3.one * _scale * scale * (1 + 0.2f*Mathf.Sin(_blinkSpeed*Time.time));
		}
	}

	void OnDestroy()
	{
		gameObject.Cleanup();
	}

	private GameObject AddIcon(GameObject prefab)
	{
		var icon = _objectPool.GetObject(prefab);
		icon.transform.parent = transform;
		icon.gameObject.Move(Vector2.zero);
		icon.SetActive(true);
		return icon;
	}

	private GameObject AddBorder(Galaxy.Star star, Color color)
	{
		var item = _objectPool.GetObject(StarBorderPrefab);
		item.transform.parent = transform;
		item.gameObject.Move(Vector2.zero);
		item.SetActive(true);
		var border = item.GetComponent<StarBorder>();
		border.Create(star, color);
		return item;
	}

	private GameObject AddStarInfo(Galaxy.Star star)
	{
		var item = AddIcon(StarInfo);
		var textMesh = item.GetComponent<TextMesh>();

		var name = star.Bookmark;
		if (string.IsNullOrEmpty(name))
			name = star.Name;

		if (star.Region.IsCaptured)
		{
			textMesh.color = ColorTable.DefaultTextColor;
			textMesh.text = _localization.GetString("$CapturedStarInfo", name, Mathf.Max(star.Level,5));
		}
		else
		{
			textMesh.color = new Color(1f,0.75f,0.5f);
			textMesh.text = _localization.GetString("$StarInfo", name, Mathf.RoundToInt(star.Region.BaseDefensePower*100) + "%");
		}

		return item;
	}

    private GameObject AddStarBookmark(Galaxy.Star star)
    {
        var item = AddIcon(StarInfo);
        var textMesh = item.GetComponent<TextMesh>();

        textMesh.color = ColorTable.DefaultTextColor;
        textMesh.text = string.IsNullOrEmpty(star.Bookmark) ? star.Name : star.Bookmark;

        return item;
    }

    private void OnStateChanged()
	{
        if (_state == State.Galaxy && _showMiniStarOnGalaxyMap)
        {
            MiniObject.SetActive(true);
            MiniObject.transform.localScale = 5*_miniObjectScale;
            StarObject.SetActive(false);
            HaloObject.SetActive(false);
        }
        else if (_state == State.Hidden || _state == State.Galaxy)
		{
			MiniObject.SetActive(false);
			StarObject.SetActive(false);
			HaloObject.SetActive(false);
		}
		else
		{
            MiniObject.transform.localScale = _miniObjectScale;
            MiniObject.SetActive(_state == State.Map);
			StarObject.SetActive(_state != State.Map);
			HaloObject.SetActive(_state != State.Map);
		}

		foreach (Transform item in transform)
			item.SendMessage("OnStateChanged", _state, SendMessageOptions.DontRequireReceiver);
	}

	public static Color GetStarColor(int id)
	{
		Random.seed = id;
		var value = Random.value;

		Color color;
		switch (Random.Range(0,3))
		{
		case 0:
			color = Color.Lerp(Color.yellow, Color.white, value);
			break;
		case 1:
			color = Color.Lerp(Color.cyan, Color.white, value);
			break;
		case 2:
		default:
			color = Color.Lerp(Color.red, Color.yellow, value);
			break;
		}

		color.r = 0.2f + color.r * 0.5f;
		color.g = 0.2f + color.g * 0.5f;
		color.b = 0.2f + color.b * 0.5f;
		color.a *= 1.5f;

		return color;
	}

	private void Start()
	{
		_mainCamera = Camera.main;
	}

	private bool _showMiniStarOnGalaxyMap;
    private Vector3 _miniObjectScale;
    private int _starId;
	private float _scale;
	private float _blinkSpeed;
	private State _state;
	private Camera _mainCamera;
}
