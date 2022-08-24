using System.Linq;
using System.Collections.Generic;
using Galaxy;
using Game.Exploration;
using GameDatabase;
using GameDatabase.Enums;
using UnityEngine;
using Zenject;

namespace StarSystem
{
	[RequireComponent(typeof(FleetObjects))]
	public class OrbitalObjects : MonoBehaviour
	{
	    [Inject] private readonly Game.Exploration.Planet.Factory _planetFactory;
	    [Inject] private readonly IDatabase _database;

        public Transform PlanetObjects;
        public Transform OrbitObjects;
        public Transform AsteroidBeltObjects;
	    public Transform SpaceStationObjects;
		public GameObject WormholeObject;
		public GameObject BeaconObject;
        public GameObject PandemicObject;

		public int MinOrbitRadius;
		public int OrbitDistance;
		public float MaxPlanetSize;
		public float StarBaseSize;
		public float WormholeSize;
		public float BeaconSize;
		public float SpaceStationSize;

		public void Initialize(Galaxy.Star star, Color color)
		{
			gameObject.SetActive(true);
			color = Color.Lerp(color, new Color(1f,1f,1f,0f), 0.6f);
			_orbitRadius = MinOrbitRadius;
			_lastAngle = 0;

		    CreateObjects(star, color);
		}
		
		public void Cleanup()
		{
            PlanetObjects.DeactivateChildren<Planet>();
            OrbitObjects.DeactivateChildren<Circle>();
            AsteroidBeltObjects.DeactivateChildren<Circle>();
            SpaceStationObjects.DeactivateChildren<StarBase>();
            WormholeObject.SetActive(false);
            BeaconObject.SetActive(false);
			PandemicObject.SetActive(false);

            _planets.Clear();
			_objects.Clear();
			GetComponent<FleetObjects>().Cleanup();
			gameObject.SetActive(false);
		}

		public bool IsActive { get { return gameObject.activeSelf; } }

		public IList<Planet> Planets { get { return _planets; } }
		public int NextOrbitRadius { get { return _orbitRadius; } }

		public int TryGetPlanet(ref Vector2 position)
		{
			for (int i = 0; i < _planets.Count; ++i)
			{
				var item = _planets[i];
				var distance = Vector2.Distance(item.transform.position, position);
				if (distance > 2*item.transform.localScale.z*transform.parent.localScale.z)
					continue;
                
				position = item.transform.localPosition;
                return System.Convert.ToInt32(_planets[i].name);
            }

			return -1;
		}

		public Galaxy.StarObjectType TryGetPointOfInterest(ref Vector2 position)
		{
			foreach (var item in _objects)
			{
				var distance = Vector2.Distance(item.Key.transform.position, position);
				if (distance > 2*MaxPlanetSize*transform.parent.localScale.z)
					continue;

				position = item.Key.transform.localPosition;
                return item.Value;
            }
            
            return Galaxy.StarObjectType.Undefined;
        }

	    private void CreateObjects(Galaxy.Star star, Color color)
	    {
            var orbitEnumerator = OrbitObjects.CreateChildren<Circle>().GetEnumerator();
            CreatePlanets(color, star.Id, orbitEnumerator);
            CreatePointsOfInterest(star, color, orbitEnumerator);
        }

        private void CreatePlanets(Color color, int starId, IEnumerator<Circle> orbitEnumerator)
		{
			_planets.Clear();

			var planets = _planetFactory.CreatePlanets(starId);
			var random = new System.Random(starId);

            var planetEnumerator = PlanetObjects.CreateChildren<Planet>().GetEnumerator();
            var asteroidBeltEnumerator = AsteroidBeltObjects.CreateChildren<Circle>().GetEnumerator();

            foreach (var item in planets)
			{
			    if (item.Type == PlanetType.Asteroids)
			    {
			        asteroidBeltEnumerator.MoveNext();
			        CreateAsteroidBelt(asteroidBeltEnumerator.Current, color);
			    }
			    else
			    {
			        planetEnumerator.MoveNext();
			        orbitEnumerator.MoveNext();
                    CreateOrbit(orbitEnumerator.Current);
                    CreatePlanet(planetEnumerator.Current, item, color, random).name = item.Index.ToString();
			    }
			}
		}

		private void CreateOrbit(Circle orbitObject)
		{
            orbitObject.transform.localPosition = Vector3.zero;
            orbitObject.transform.localScale = Vector3.one*(_orbitRadius + orbitObject.Thickness/2);
		}

		private Planet CreatePlanet(Planet planetObject, Game.Exploration.Planet model, Color color, System.Random random)
		{
            planetObject.Image.sprite = model.Icon;

            planetObject.transform.localScale = Vector3.one*0.5f*(model.Size + 1f)*MaxPlanetSize;
			_lastAngle += 60 + random.Next(240);
			var localRotation = random.Next(360);

            planetObject.transform.localPosition = RotationHelpers.Direction(_lastAngle)*_orbitRadius;
            planetObject.transform.localEulerAngles = new Vector3(0,0,localRotation);
            planetObject.Mask.color = color;
            planetObject.Mask.transform.localEulerAngles = new Vector3(0,0,_lastAngle - localRotation);
			
			_planets.Add(planetObject);
			_orbitRadius += OrbitDistance;

			return planetObject;
		}

		private StarBase CreateStarBase(StarBase starBaseObject, float size, Color color, System.Random random)
		{
			starBaseObject.Image.color = color;
			starBaseObject.transform.localScale = Vector3.one*StarBaseSize;
			_lastAngle += 60 + random.Next(240);

			starBaseObject.transform.localPosition = RotationHelpers.Direction(_lastAngle)*_orbitRadius;
			_orbitRadius += OrbitDistance;
			
			return starBaseObject;
		}

		void CreateAsteroidBelt(Circle gameObject, Color color)
		{
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one*(_orbitRadius + gameObject.Thickness/2);
			_orbitRadius += OrbitDistance;
		}

		private void CreateCapital(StarBase starBaseObject, GameModel.Region region, Color color)
		{
			var starbase = CreateStarBase(starBaseObject, StarBaseSize, color, new System.Random(region.GetHashCode()));

			if (!region.IsCaptured)
				GetComponent<FleetObjects>().CreateShips(Model.Factories.Fleet.Capital(region, _database).Ships.Take(20), starbase.transform.localPosition);

			_objects.Add(starbase.gameObject, Galaxy.StarObjectType.StarBase);
		}

		private void CreateBoss(Galaxy.StarContent.Boss.Facade boss, Color color)
		{
			var ships = GetComponent<FleetObjects>();
            var fleet = boss.CreateFleet();
			var flagship = fleet.Ships.FirstOrDefault(item => item.Model.Category == ShipCategory.Flagship) ?? fleet.Ships.First();

			var random = new System.Random(boss.GetHashCode());
			_lastAngle += 60 + random.Next(240);
			var position = RotationHelpers.Direction(_lastAngle)*_orbitRadius;            
			ships.CreateShips(fleet.Ships.Where(item => item.Model.Category != ShipCategory.Flagship), position);

		    var flagshipObject = ships.CreateFlagship(_database.GetShip(flagship.Model.Id), position, random.Next(360));
		    _objects.Add(flagshipObject.gameObject, Galaxy.StarObjectType.Boss);

            _orbitRadius += OrbitDistance;
		}

		private void CreateRuins(StarBase starBaseObject, Galaxy.StarContent.Ruins.Facade ruins, Color color)
		{
			var starbase = CreateStarBase(starBaseObject, StarBaseSize, color, new System.Random(ruins.GetHashCode()));
			GetComponent<FleetObjects>().CreateShips(ruins.CreateFleet().Ships, starbase.transform.localPosition);
			
			_objects.Add(starbase.gameObject, Galaxy.StarObjectType.Ruins);
		}

        private void CreateXmas(StarBase starBaseObject, Galaxy.StarContent.XmasTree.Facade xmas, Color color)
        {
            var starbase = CreateStarBase(starBaseObject, StarBaseSize, color, new System.Random(xmas.GetHashCode()));
            GetComponent<FleetObjects>().CreateShips(xmas.CreateFleet().Ships, starbase.transform.localPosition);

            _objects.Add(starbase.gameObject, Galaxy.StarObjectType.Xmas);
        }

		private void CreateCommonObject(StarBase starBaseObject, Galaxy.StarObjectType pointOfInterest, Color color)
		{
			var random = new System.Random(GetHashCode());
			var starbase = CreateStarBase(starBaseObject, SpaceStationSize, color, random);
			_objects.Add(starbase.gameObject, pointOfInterest);
		}

        private void CreateCommonObject(GameObject spaceObject, StarObjectType type)
        {
            var random = new System.Random(GetHashCode());
            spaceObject.transform.localScale = Vector3.one * WormholeSize;
            _lastAngle += 60 + random.Next(240);

            spaceObject.transform.localPosition = RotationHelpers.Direction(_lastAngle) * _orbitRadius;
            _orbitRadius += OrbitDistance;

            _objects.Add(spaceObject, type);
        }

		private void CreateEventObject(GameObject beaconGameObject)
		{
			var random = new System.Random(GetHashCode());

			beaconGameObject.transform.localScale = Vector3.one*BeaconSize;
			_lastAngle += 60 + random.Next(240);			
			beaconGameObject.transform.localPosition = RotationHelpers.Direction(_lastAngle)*_orbitRadius;
			_orbitRadius += OrbitDistance;
			
			_objects.Add(beaconGameObject, Galaxy.StarObjectType.Event);
		}

		private void CreatePointsOfInterest(Galaxy.Star star, Color color, IEnumerator<Circle> orbitEnumerator)
		{
		    var objects = star.Objects;
		    var starBaseEnumerator = SpaceStationObjects.CreateChildren<StarBase>().GetEnumerator();

		    if (star.HasStarBase)
		    {
		        orbitEnumerator.MoveNext();
                CreateOrbit(orbitEnumerator.Current);
		        starBaseEnumerator.MoveNext();
		        CreateCapital(starBaseEnumerator.Current, star.Region, color);
		    }

		    if (objects.Contain(StarObjectType.Boss) && StarObjectType.Boss.IsActive(star))
		        CreateBoss(star.Boss, color);
		    if (objects.Contain(Galaxy.StarObjectType.Wormhole))
		    {
                WormholeObject.SetActive(true);
		        CreateCommonObject(WormholeObject, StarObjectType.Wormhole);
		    }
		    if (objects.Contain(Galaxy.StarObjectType.Ruins) && StarObjectType.Ruins.IsActive(star))
		    {
                orbitEnumerator.MoveNext();
                CreateOrbit(orbitEnumerator.Current);
                starBaseEnumerator.MoveNext();
                CreateRuins(starBaseEnumerator.Current, star.Ruins, color);
            }
            if (objects.Contain(Galaxy.StarObjectType.Xmas) && StarObjectType.Xmas.IsActive(star))
            {
                orbitEnumerator.MoveNext();
                CreateOrbit(orbitEnumerator.Current);
                starBaseEnumerator.MoveNext();
                CreateXmas(starBaseEnumerator.Current, star.Xmas, color);
            }
            if (objects.Contain(Galaxy.StarObjectType.Arena) && StarObjectType.Arena.IsActive(star))
		    {
                starBaseEnumerator.MoveNext();
                CreateCommonObject(starBaseEnumerator.Current, Galaxy.StarObjectType.Arena, color);
		    }
		    if (objects.Contain(Galaxy.StarObjectType.Military) && StarObjectType.Military.IsActive(star))
		    {
                starBaseEnumerator.MoveNext();
                CreateCommonObject(starBaseEnumerator.Current, Galaxy.StarObjectType.Military, color);
		    }
		    if (objects.Contain(Galaxy.StarObjectType.Challenge) && StarObjectType.Challenge.IsActive(star))
		    {
                starBaseEnumerator.MoveNext();
                CreateCommonObject(starBaseEnumerator.Current, Galaxy.StarObjectType.Challenge, color);
		    }
		    if (objects.Contain(Galaxy.StarObjectType.Survival) && StarObjectType.Survival.IsActive(star))
		    {
                starBaseEnumerator.MoveNext();
                CreateCommonObject(starBaseEnumerator.Current, Galaxy.StarObjectType.Survival, color);
		    }
		    if (objects.Contain(Galaxy.StarObjectType.BlackMarket) && StarObjectType.BlackMarket.IsActive(star))
		    {
                starBaseEnumerator.MoveNext();
                CreateCommonObject(starBaseEnumerator.Current, Galaxy.StarObjectType.BlackMarket, color);
		    }
            if (objects.Contain(Galaxy.StarObjectType.Hive) && StarObjectType.Hive.IsActive(star))
            {
                PandemicObject.SetActive(true);
                CreateCommonObject(PandemicObject, StarObjectType.Hive);
            }
			if (objects.Contain(Galaxy.StarObjectType.Event) && StarObjectType.Event.IsActive(star))
		    {
                BeaconObject.SetActive(true);
		        CreateEventObject(BeaconObject);
		    }
		}

		private int _orbitRadius;
		private int _lastAngle;
		private List<Planet> _planets = new List<Planet>();
		private Dictionary<GameObject, Galaxy.StarObjectType> _objects = new Dictionary<GameObject, Galaxy.StarObjectType>();
	}
}
