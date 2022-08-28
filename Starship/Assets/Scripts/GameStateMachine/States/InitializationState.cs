using System;
using System.Linq;
using Constructor;
using Constructor.Ships;
using Diagnostics;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Database;
using GameServices.LevelManager;
using GameServices.Settings;
using Services.Account;
using Services.Storage;
using Session;
using UnityEngine;
using Zenject;

namespace GameStateMachine.States
{
    public class InitializationState : BaseState
    {
        [Inject]
        public InitializationState(IStateMachine stateMachine, GameStateFactory stateFactory, ILevelLoader levelLoader, 
            ISessionData sessionData, IDataStorage localStorage, GameSettings settings, IAccount account, IDatabase database, ITechnologies technologies, 
            ISessionData session, IDebugManager debugManager)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _sessionData = sessionData;
            _technologies = technologies;
            _database = database;
            _localStorage = localStorage;
            _settings = settings;
            _account = account;
            _debugManager = debugManager;
        }

        public override StateType Type { get { return StateType.Initialization; } }

        protected override void OnLoad()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif

#if UNITY_STANDALONE
            Application.runInBackground = _settings.RunInBackground;
            QualitySettings.SetQualityLevel(1);
#else
            QualitySettings.SetQualityLevel(_settings.LowQualityMode ? 0 : 1);
#endif

            Debug.Log (SystemInfo.operatingSystem);
            Debug.Log (SystemInfo.deviceModel);
            Debug.Log (SystemInfo.deviceType.ToString ());
            Debug.Log (SystemInfo.deviceName);

            var mod = _settings.ActiveMod;
            string error;
            if (!string.IsNullOrEmpty(mod) && _database.TryLoad(mod, out error))
            {
                Debug.Log("Mod loaded - " + mod);
            }
            else
            {
                mod = string.Empty;
                _database.LoadDefault();
            }
        }

        protected override void OnActivate()
		{
		    if (_database.IsEditable)
		    {
		        Debug.Log("Checking ships...");

		        //var ships = Resources.LoadAll<Constructor.ShipBuild> ("Prefabs/Constructor/Builds");
		        //var drones = Resources.LoadAll<Constructor.ShipBuild> ("Prefabs/Constructor/DroneBuilds");
		        foreach (var item in _database.ShipBuildList)
		        {
		            // TODO: move to database
		            var ship = new CommonShip(item);
		            var debug = _debugManager.CreateLog(item.Id.ToString());
		            var layout = new Constructor.ShipLayout(item.Ship.Layout, item.Ship.Barrels, ship.Components, debug);
		            if (layout.Components.Count() != ship.Components.Count)
		            {
		                Debug.LogError("invalid ship layout: " + item.Id);
		                Debug.Break();
		            }

		            if (item.Ship.ShipCategory != ShipCategory.Special &&
		                item.Ship.ShipCategory != ShipCategory.Starbase && !item.NotAvailableInGame &&
		                !ShipValidator.IsShipViable(new CommonShip(item), _database.ShipSettings))
		            {
		                Debug.LogError("invalid build: " + item.Id);
		                Debug.Break();
		            }
		        }

		        var companions = _database.SatelliteBuildList; //Resources.LoadAll<Constructor.CompanionBuildWrapper> ("Prefabs/Constructor/CompanionBuilds");
		        foreach (var item in companions)
		        {
		            var debug = _debugManager.CreateLog(item.Id.ToString());
		            var components = item.Components
		                .Select<InstalledComponent, IntegratedComponent>(ComponentExtensions.FromDatabase).ToArray();
		            var layout = new ShipLayout(item.Satellite.Layout, item.Satellite.Barrels, components, debug);
		            if (layout.Components.Count() != components.Length)
		            {
		                Debug.LogError("invalid companion layout: " + item.Id);
		                Debug.Break();
		            }
		        }

		        Debug.Log("Checking techs...");

		        foreach (var tech in _database.TechnologyList)
		        {
		            var index = tech.Dependencies.IndexOf(null);
                    if (index >= 0)
		            {
		                _debugManager.CreateLog(tech.Id.ToString()).Write("unknown dependency - " + index);
		                Debug.LogError("invalid tech: " + tech.Id);
		            }
                }

		        Debug.Log("...done");
		    }

		    Debug.Log("InitializationState: signin - " + _settings.SignedIn);
			if (_settings.SignedIn)
			{
				_account.SignIn();
			}

		    if (_localStorage.TryLoad(_sessionData, _database.Id))
		        Debug.Log("Saved game loaded");
		    else if (_database.IsEditable && _localStorage.TryImportOriginalSave(_sessionData, _database.Id))
		        Debug.Log("Original saved game imported");
		    else
		        _sessionData.CreateNewGame(_database.Id);

            StateMachine.LoadState(StateFactory.CreateMainMenuState());
        }

        protected override LevelName RequiredLevel { get { return LevelName.CommonGui; } }

        protected override void OnSuspend(StateType newState)
        {
            throw new InvalidOperationException();
        }

        protected override void OnResume(StateType oldState)
        {
            throw new InvalidOperationException();
        }

        private readonly ISessionData _sessionData;
        private readonly IDatabase _database;
        private readonly ITechnologies _technologies;
        private readonly IDataStorage _localStorage;
        private readonly GameSettings _settings;
        private readonly IAccount _account;
        private readonly IDebugManager _debugManager;

        public class Factory : Factory<InitializationState> { }
    }
}
