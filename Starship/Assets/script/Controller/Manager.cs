using System.Linq;
using UnityEngine;
using Combat.Ai;
using Combat.Component.Unit.Classification;
using Combat.Factory;
using Combat.Scene;
using Combat.Unit;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Settings;
using GameStateMachine.States;
using Gui.Combat;
using Services.Audio;
using Services.Messenger;
using Services.ObjectPool;
using Services.Reources;
using Zenject;

namespace Controller
{
	public class Manager : MonoBehaviour
	{
        [Inject] private readonly GameSettings _gameSettings;
	    [Inject] private readonly ExitSignal.Trigger _exitTrigger;
	    [Inject] private readonly IScene _scene;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly ISoundPlayer _soundPlayer;
	    [Inject] private readonly IAiManager _aiManager;
	    [Inject] private readonly Combat.Settings _settings;
	    [Inject] private readonly IDatabase _database;
	    [Inject] private readonly IResourceLocator _resourceLocator;
	    [Inject] private readonly ShipFactory _shipFactory;
        [Inject] private readonly ShipControlsPanel _shipControlsPanel;
	    [Inject] private readonly IKeyboard _keyboard;

        public GameObject Background;
		public GameObject SettingsPanel;
		public ViewModel.PanelController StopButton;

	    [Inject]
	    private void Initialize(IMessenger messenger)
	    {
            messenger.AddListener(EventType.EscapeKeyPressed, OnEscapeKeyPressed);

            SettingsPanel.SetActive(true);
            Background.SetActive(false);
        }

        public void Simulate()
		{
			var playerShip = _scene.PlayerShip;
			if (!playerShip.IsActive())
				playerShip = _shipFactory.CreateShip(new CommonShip(_database.GetShipBuild(new ItemId<ShipBuild>(226))).CreateBuilder().Build(_database.ShipSettings), 
                    new KeyboardController.Factory(_keyboard), UnitSide.Player, Vector2.zero, 0f);

            if (!_scene.EnemyShip.IsActive())
                _shipFactory.CreateShip(new EnemyShip(_database.GetShipBuild(new ItemId<ShipBuild>(218))).CreateBuilder().Build(_database.ShipSettings),
                    new Computer.Factory(_scene, 10), UnitSide.Enemy, new Vector2(5,5), 0);

            _shipControlsPanel.Load(playerShip);

			Background.SetActive(true);
			SettingsPanel.SetActive(false);
			StopButton.Open();
		}

		public void Stop()
		{
			foreach (var ship in _scene.Ships.Items)
				ship.Vanish();

            _shipControlsPanel.Load(null);

            Background.SetActive(false);
            SettingsPanel.SetActive(true);
			StopButton.Close();
		}

		public void Exit()
		{
            _exitTrigger.Fire();
        }

		private void OnEscapeKeyPressed()
		{
            if (_scene.Ships.Items.Any(item => item.IsActive()))
		        Stop();
		}
	}
}
