using Database.Legacy;
using GameDatabase;
using UnityEngine;
using GameServices.Player;
using Services.Messenger;
using Services.Reources;
using Zenject;

namespace StarSystem
{
	public class StarSystem : MonoBehaviour
	{
	    [Inject] private readonly MotherShip _player;
        [Inject] private readonly PlayerSkills _playerSkills;
        [Inject] private readonly IMessenger _messenger;
	    [Inject] private readonly IDatabase _database;
	    [Inject] private readonly IResourceLocator _resourceLocator;

		public FleetObjects PlayerFleet;
		public FleetObjects EnemyFleet;
		public OrbitalObjects Objects;

		public void Initialize(Galaxy.Star star, Color color)
		{
            try
            {
			    gameObject.SetActive(true);
			    Objects.Initialize(star, color);
			    CreatePlayerShip(Objects.NextOrbitRadius + 2, star.Id);
			    CreateGuardian(star);

				//var playerPosition = (Vector2)_playerShip.transform.position;
				//SelectObject(ref playerPosition, false);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

		public void Cleanup()
		{
			Objects.Cleanup();
			EnemyFleet.Cleanup();
			gameObject.SetActive(false);
		}		

		public event System.Action<Vector2> MovedEvent = position => {};

		public void OnClick(Vector2 position)
		{
			if (SelectObject(ref position, true) && Vector2.Distance(_playerShip.transform.localPosition, position) > 0.1f)
			{
				MoveTo(position);

                if (EnemyFleet.AreEnemiesNearby(position, transform.localScale.z))
				{
					EnemyFleet.Escape(position, Objects.Planets);
					_player.CurrentStar.Occupant.Attack();
				}
            }
        }
        
        public bool IsActive { get { return gameObject.activeSelf && Objects.IsActive; } }

		private bool SelectObject(ref Vector2 position, bool existingOnly)
		{
			var planetId = Objects.TryGetPlanet(ref position);
			if (planetId >= 0)
			{
                _messenger.Broadcast(EventType.ArrivedToPlanet, planetId);
				return true;
			}
			
			var pointOfInterest = Objects.TryGetPointOfInterest(ref position);
			if (pointOfInterest != Galaxy.StarObjectType.Undefined)
            {
                _messenger.Broadcast(EventType.ArrivedToObject, pointOfInterest);
                return true;
            }

			if (!existingOnly)
                _messenger.Broadcast(EventType.ArrivedToObject, Galaxy.StarObjectType.Undefined);

			return false;
		}
        
        private void MoveTo(Vector2 target)
		{
			_playerShip.MoveTo(target);
			MovedEvent.Invoke((Vector2)transform.localPosition + 0.75f*target*transform.localScale.z);
        }

		private void Start()
		{
            _messenger.AddListener(EventType.GuardianDefeated, OnGuardianDefeated);
            _messenger.AddListener<int>(EventType.PlayerPositionChanged, OnPlayerPositionChanged);
        }
        
        private void CreatePlayerShip(int distance, int seed)
		{
			if (_playerShip == null)
			{
				var angle = new System.Random(seed).Next(360);
				var position = distance*RotationHelpers.Direction(angle+180);
				_playerShip = PlayerFleet.CreateFlagship(_database.GetShip(LegacyShipNames.GetId("mothership")), position, angle);

				_playerShip.EnginePower = _playerSkills.MainEnginePower;
			}

			MovedEvent((Vector2)transform.localPosition + 0.75f*_playerShip.Position*transform.localScale.z);
		}

		private void CreateGuardian(Galaxy.Star star)
		{
            if (!star.Occupant.IsExists)
                return;

			EnemyFleet.CreateShips(star.Occupant.CreateFleet().Ships, new System.Random(star.Id), Objects.Planets);

			if (star.Occupant.IsAggressive)
				EnemyFleet.MoveTo(_playerShip.Position);
		}

		private void OnGuardianDefeated()
		{
			EnemyFleet.Cleanup();
		}

		private void OnPlayerPositionChanged(int starId)
		{
			PlayerFleet.Cleanup();
			_playerShip = null;
		}

		private Ship _playerShip;
	}
}
