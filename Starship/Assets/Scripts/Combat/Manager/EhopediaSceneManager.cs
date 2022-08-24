using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Ai;
using Combat.Collision;
using Combat.Component.Body;
using Combat.Component.Triggers;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Factory;
using Combat.Scene;
using Combat.Unit;
using Combat.Unit.Object;
using Economy.Products;
using Game.Exploration;
using GameStateMachine.States;
using Services.Messenger;
using GameDatabase;
using GameServices;
using GameServices.Economy;
using GameServices.Player;
using Services.Gui;
using Services.Localization;
using Services.ObjectPool;
using Services.Reources;
using Services.Unity;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using IShip = Combat.Component.Ship.IShip;


namespace Combat.Manager
{
    public class EhopediaSceneManager : ITickable, IInitializable, IDisposable
    {
        [Inject]
        private EhopediaSceneManager(
            IMessenger messenger)
        {
            _messenger = messenger;

            _messenger.AddListener<IShip>(EventType.CombatShipCreated, OnShipCreated);
            _messenger.AddListener<IShip>(EventType.CombatShipDestroyed, OnShipDestroyed);
            //_messenger.AddListener(EventType.Surrender, CancelCombat);
        }

        [Inject] private readonly IScene _scene;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly IAiManager _aiManager;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly IResourceLocator _resourceLocator;
        //[Inject] private readonly Gui.Combat.ShipControlsPanel _shipControlsPanel;
        [Inject] private readonly ShipFactory _shipFactory;
        [Inject] private readonly SpaceObjectFactory _spaceObjectFactory;
        [Inject] private readonly EffectFactory _effectFactory;
        [Inject] private readonly IGuiManager _guiManager;
        [Inject] private readonly GameFlow _gameFlow;
        //[Inject] private readonly IKeyboard _keyboard;
        //[Inject] private readonly ILocalization _localization;
        //[Inject] private readonly LootGenerator _lootGenerator;
        //[Inject] private readonly ExitSignal.Trigger _exitTrigger;

        [Inject] private readonly Settings _settings;
        //[Inject] private readonly Gui.Combat.RadarPanel _radarPanel;

        private void OnShipCreated(IShip ship)
        {
            //if (ship.Type.Class != UnitClass.Ship)
            //    return;

            //switch (ship.Type.Side)
            //{
            //    case UnitSide.Player:
            //        _shipControlsPanel.Load(ship);
            //        break;
            //    case UnitSide.Enemy:
            //        _radarPanel.Add(ship);
            //        break;
            //}
        }

        private void OnShipDestroyed(IShip ship)
        {
            //if (ship.Type.Class != UnitClass.Ship)
            //    return;

            //if (ship.Type.Side == UnitSide.Player)
            //    DelayedExit();
        }

        public void Tick()
        {
        }

        //private void ApplyExperience()
        //{
        //    var player = _scene.PlayerShip;
        //    var damageDealt = player.Stats.TotalDamageDealt;
        //    var experience = _playerFleet.PlayerShip.CalculateExperience(damageDealt * _playerSkills.ExperienceMultiplier, _sceneMap.Level);
        //    if (experience > 0)
        //        _expReceivedTrigger.Fire(new ShipExpPoints(_playerFleet.PlayerShip, experience));
        //}

        private void KillThemAll()
        {
            foreach (var ship in _scene.Ships.Items)
                if (ship.Type.Side.IsEnemy(UnitSide.Player))
                    ship.Affect(new Impact { Effects = CollisionEffect.Destroy });
        }

        public void Initialize()
        {
            var color = new Color(0.1f, 0.2f, 0.3f);
            var position = new Vector2(-10, -10);
            var size = 30;
            _spaceObjectFactory.CreatePlanet(position, size, color);
        }

        public void Dispose()
        {
        }

        private readonly IMessenger _messenger;
    }
}
