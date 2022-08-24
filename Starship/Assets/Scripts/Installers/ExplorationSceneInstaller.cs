using Combat;
using Combat.Ai;
using Combat.Collision.Manager;
using Combat.Component.Triggers;
using Combat.Factory;
using Combat.Manager;
using Combat.Scene;
using Combat.Services;
using Game.Exploration;
using Gui.Combat;
using Services.ObjectPool;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ExplorationSceneInstaller : MonoInstaller<ExplorationSceneInstaller>
    {
        [SerializeField] private ShipStatsPanel _playerStatsPanel;
        [SerializeField] private ShipStatsPanel _enemyStatsPanel;
        [SerializeField] private ShipControlsPanel _shipControlsPanel;
        [SerializeField] private RadarPanel _radarPanel;
        [SerializeField] private Settings _settings;
        [SerializeField] private TrailRendererPool _trailRendererPool;

        [Inject] private readonly GameStateMachine.IStateMachine _stateMachine;

        public override void InstallBindings()
        {
            Container.BindAllInterfacesAndSelf<ExplorationData>().To<ExplorationData>().AsSingle().NonLazy();
            Container.BindAllInterfacesAndSelf<ExplorationSceneManager>().To<ExplorationSceneManager>().AsSingle().NonLazy();
            Container.BindAllInterfaces<ExplorationViewRect>().To<ExplorationViewRect>().AsTransient();
            Container.BindAllInterfaces<Scene>().To<Scene>().AsSingle().WithArguments(new SceneSettings { AreaWidth = 1000, AreaHeight = 1000, PlayerAlwaysInCenter = true }).NonLazy();
            Container.BindAllInterfaces<CollisionManager>().To<CollisionManager>().AsSingle();
            Container.BindAllInterfaces<AiManager>().To<AiManager>().AsSingle().NonLazy();
            Container.Bind<ShipStatsPanel>().FromInstance(_playerStatsPanel).When(context => context.MemberName.Contains("player"));
            Container.Bind<ShipStatsPanel>().FromInstance(_enemyStatsPanel).When(context => context.MemberName.Contains("enemy"));
            Container.Bind<ShipControlsPanel>().FromInstance(_shipControlsPanel);
            Container.Bind<RadarPanel>().FromInstance(_radarPanel);
            Container.Bind<Settings>().FromInstance(_settings);
            Container.Bind<WeaponFactory>().AsSingle();
            Container.Bind<ShipFactory>().AsSingle().WithArguments(new ShipFactory.Settings { Shadows = true, StaticWrecks = true });
            Container.Bind<SpaceObjectFactory>().AsSingle();
            Container.Bind<DeviceFactory>().AsSingle();
            Container.Bind<DroneBayFactory>().AsSingle();
            Container.Bind<SatelliteFactory>().AsSingle();
            Container.Bind<EffectFactory>().AsSingle();
            Container.BindAllInterfacesAndSelf<IObjectPool>().To<GameObjectPool>().FromGameObject().AsSingle();
            Container.Bind<TrailRendererPool>().FromInstance(_trailRendererPool);
            Container.Bind<GameObjectFactory>();
            Container.BindAllInterfaces<Keyboard>().To<Keyboard>().AsSingle();
        }
    }
}
