using Combat;
using Combat.Ai;
using Combat.Collision.Manager;
using Combat.Factory;
using Combat.Scene;
using Combat.Services;
using Gui.Combat;
using Services.ObjectPool;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ConfigSceneInstaller : MonoInstaller<ConfigSceneInstaller>
    {
        [SerializeField] private Settings _settings;
        [SerializeField] private ShipControlsPanel _shipControlsPanel;
        [SerializeField] private TrailRendererPool _trailRendererPool;

        public override void InstallBindings()
        {
            Container.Bind<Settings>().FromInstance(_settings);
            Container.BindAllInterfaces<ViewRect>().To<ViewRect>().AsTransient();
            Container.BindAllInterfaces<Scene>().To<Scene>().AsSingle().WithArguments(new SceneSettings { AreaWidth = 200, AreaHeight = 200 }).NonLazy();
            Container.BindAllInterfaces<CollisionManager>().To<CollisionManager>().AsSingle();
            Container.BindAllInterfaces<AiManager>().To<AiManager>().AsSingle().NonLazy();
            Container.Bind<ShipControlsPanel>().FromInstance(_shipControlsPanel);
            Container.Bind<WeaponFactory>().AsSingle();
            Container.Bind<ShipFactory>().AsSingle().WithArguments(new ShipFactory.Settings());
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
