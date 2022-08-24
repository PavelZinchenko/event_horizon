using Diagnostics;
using GameDatabase;
using GameServices;
using GameServices.Gui;
using GameServices.LevelManager;
using GameServices.Settings;
using Services.Account;
using Services.Audio;
using Services.Gui;
using Services.InternetTime;
using Services.Localization;
using Services.Messenger;
using Services.ObjectPool;
using Services.Reources;
using Services.Storage;
using Services.Unity;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ServicesInstaller : MonoInstaller<ServicesInstaller>
    {
        [SerializeField] private MusicPlayer _musicPlayer;
        [SerializeField] private SoundPlayer _soundPlayer;

        public override void InstallBindings()
        {
#if EDITOR_MODE
            InstallEditorBindings();
            return;
#endif
            Container.Bind<IResourceLocator>().FromPrefabResource("ResourceLocator").AsSingle().NonLazy();

            Container.BindAllInterfaces<GameDatabase.Database>().To<GameDatabase.Database>().AsSingle().NonLazy();
            Container.BindSignal<GameDatabaseLoadedSignal>();
            Container.BindTrigger<GameDatabaseLoadedSignal.Trigger>();
            Container.Bind<DatabaseStatistics>().AsSingle().NonLazy();
            Container.BindAllInterfaces<DebugManager>().To<DebugManager>().AsSingle();

            Container.Bind<IMessenger>().To<Messenger>().AsSingle();

            Container.Bind<ILocalization>().To<LocalizationManager>().AsSingle();

// #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
//             Container.BindAllInterfaces<DiscordController>().To<DiscordController>().AsSingle().NonLazy();
// #endif

            Container.Bind<ICoroutineManager>().To<CoroutineManager>().FromGameObject().AsSingle();

            Container.BindAllInterfacesAndSelf<GameSettings>().To<GameSettings>().AsSingle().NonLazy();

            Container.BindAllInterfaces<GuiManager>().To<GuiManager>().AsSingle().NonLazy();
            Container.BindSignal<WindowOpenedSignal>();
            Container.BindTrigger<WindowOpenedSignal.Trigger>();
            Container.BindSignal<WindowClosedSignal>();
            Container.BindTrigger<WindowClosedSignal.Trigger>();
            Container.BindSignal<EscapeKeyPressedSignal>();
            Container.BindTrigger<EscapeKeyPressedSignal.Trigger>();

            Container.BindAllInterfacesAndSelf<IObjectPool>().To<GameObjectPool>().FromGameObject().AsSingle();
            Container.Bind<GameObjectFactory>();

            Container.BindAllInterfaces<MusicPlayer>().To<MusicPlayer>().FromInstance(_musicPlayer);
            Container.BindAllInterfaces<SoundPlayer>().To<SoundPlayer>().FromInstance(_soundPlayer);

#if !UNITY_EDITOR
            //Container.BindAllInterfaces<FreezeDetector>().To<FreezeDetector>().AsSingle().NonLazy();
#endif

            Container.Bind<PrefabCache>().To<PrefabCache>().FromGameObject().AsSingle();

            Container.BindAllInterfacesAndSelf<InternetTimeService>().To<InternetTimeService>().AsSingle().NonLazy();
            Container.BindSignal<ServerTimeReceivedSignal>();
            Container.BindTrigger<ServerTimeReceivedSignal.Trigger>();
            

#if UNITY_EDITOR
            Container.BindAllInterfaces<EditorModeAccount>().To<EditorModeAccount>().AsSingle();
#else
            Container.BindAllInterfaces<EmptyAccount>().To<EmptyAccount>().AsSingle();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            Container.BindAllInterfaces<AndroidLocalStorage>().To<AndroidLocalStorage>().AsSingle();
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
            Container.BindAllInterfaces<MacLocalStorage>().To<MacLocalStorage>().AsSingle();
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Container.BindAllInterfaces<WindowsLocalStorage>().To<WindowsLocalStorage>().AsSingle();
#else
            Container.BindAllInterfacesAndSelf<LocalStorage>().To<LocalStorage>().AsSingle();
#endif

            Container.BindAllInterfaces<EmptyCloudStorage>().To<EmptyCloudStorage>().AsSingle();
            Container.Bind<ILevelLoader>().To<LevelLoader>().AsSingle();

            Container.BindSignal<CloudStorageStatusChangedSignal>();
            Container.BindTrigger<CloudStorageStatusChangedSignal.Trigger>();
            Container.BindSignal<CloudLoadingCompletedSignal>();
            Container.BindTrigger<CloudLoadingCompletedSignal.Trigger>();
            Container.BindSignal<CloudSavingCompletedSignal>();
            Container.BindTrigger<CloudSavingCompletedSignal.Trigger>();
            Container.BindSignal<CloudOperationFailedSignal>();
            Container.BindTrigger<CloudOperationFailedSignal.Trigger>();
            Container.BindSignal<AccountStatusChangedSignal>();
            Container.BindTrigger<AccountStatusChangedSignal.Trigger>();
            Container.BindSignal<CloudSavedGamesReceivedSignal>();
            Container.BindTrigger<CloudSavedGamesReceivedSignal.Trigger>();

            Container.BindSignal<SceneBeforeUnloadSignal>();
            Container.BindTrigger<SceneBeforeUnloadSignal.Trigger>();
            Container.BindSignal<SceneLoadedSignal>();
            Container.BindTrigger<SceneLoadedSignal.Trigger>();
            Container.BindSignal<GamePausedSignal>();
            Container.BindTrigger<GamePausedSignal.Trigger>();
            Container.BindSignal<ShowMessageSignal>();
            Container.BindTrigger<ShowMessageSignal.Trigger>();
            Container.BindSignal<ShowDebugMessageSignal>();
            Container.BindTrigger<ShowDebugMessageSignal.Trigger>();
        }

        private void InstallEditorBindings()
        {
            Container.Bind<IResourceLocator>().FromPrefabResource("ResourceLocator").AsSingle().NonLazy();

            Container.BindAllInterfaces<GameDatabase.Database>().To<GameDatabase.Database>().AsSingle().NonLazy();
            Container.BindSignal<GameDatabaseLoadedSignal>();
            Container.BindTrigger<GameDatabaseLoadedSignal.Trigger>();

            Container.Bind<IMessenger>().To<Messenger>().AsSingle();
            Container.Bind<ILocalization>().To<LocalizationManager>().AsSingle();
            Container.Bind<ICoroutineManager>().To<CoroutineManager>().FromGameObject().AsSingle();

            Container.BindAllInterfaces<MusicPlayer>().To<MusicPlayer>().FromInstance(_musicPlayer);
            Container.BindAllInterfaces<SoundPlayer>().To<SoundPlayer>().FromInstance(_soundPlayer);
            Container.BindAllInterfacesAndSelf<GameSettings>().To<GameSettings>().AsSingle().NonLazy();

            Container.BindAllInterfaces<GuiManager>().To<GuiManager>().AsSingle().NonLazy();
            Container.BindSignal<WindowOpenedSignal>();
            Container.BindTrigger<WindowOpenedSignal.Trigger>();
            Container.BindSignal<WindowClosedSignal>();
            Container.BindTrigger<WindowClosedSignal.Trigger>();
            Container.BindSignal<EscapeKeyPressedSignal>();
            Container.BindTrigger<EscapeKeyPressedSignal.Trigger>();

            Container.Bind<ILevelLoader>().To<LevelLoader>().AsSingle();
            Container.BindSignal<SceneBeforeUnloadSignal>();
            Container.BindTrigger<SceneBeforeUnloadSignal.Trigger>();
            Container.BindSignal<SceneLoadedSignal>();
            Container.BindTrigger<SceneLoadedSignal.Trigger>();

            Container.Bind<PrefabCache>().To<PrefabCache>().FromGameObject().AsSingle();

            Container.BindAllInterfacesAndSelf<IObjectPool>().To<GameObjectPool>().FromGameObject().AsSingle();
            Container.Bind<GameObjectFactory>();

            Container.BindSignal<GamePausedSignal>();
            Container.BindTrigger<GamePausedSignal.Trigger>();
            Container.BindSignal<ShowMessageSignal>();
            Container.BindTrigger<ShowMessageSignal.Trigger>();
            Container.BindSignal<ShowDebugMessageSignal>();
            Container.BindTrigger<ShowDebugMessageSignal.Trigger>();

            Container.BindSignal<CloudStorageStatusChangedSignal>();
            Container.BindTrigger<CloudStorageStatusChangedSignal.Trigger>();
            Container.BindSignal<CloudLoadingCompletedSignal>();
            Container.BindTrigger<CloudLoadingCompletedSignal.Trigger>();
            Container.BindSignal<CloudSavingCompletedSignal>();
            Container.BindTrigger<CloudSavingCompletedSignal.Trigger>();
            Container.BindSignal<CloudOperationFailedSignal>();
            Container.BindTrigger<CloudOperationFailedSignal.Trigger>();
            Container.BindSignal<AccountStatusChangedSignal>();
            Container.BindTrigger<AccountStatusChangedSignal.Trigger>();
            Container.BindSignal<CloudSavedGamesReceivedSignal>();
            Container.BindTrigger<CloudSavedGamesReceivedSignal.Trigger>();
        }
    }
}
