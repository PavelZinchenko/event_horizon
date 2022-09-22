using UniRx;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

namespace GameServices.LevelManager
{
    public class LevelLoader : ILevelLoader
    {
        [Inject] private readonly SceneBeforeUnloadSignal.Trigger _sceneBeforeUnloadTrigger;
        [Inject] private readonly SceneLoadedSignal.Trigger _sceneLoadedTrigger;

        public void ReloadLevel(System.Action onCompleted = null, System.Action<DiContainer> installBindingsAction = null)
        {
            var level = Current;
            Current = LevelName.Undefined;
            LoadLevel(level, onCompleted, installBindingsAction);
        }

        public bool IsLoading { get; private set; }

        public LevelName Current { get; private set; } = LevelName.Undefined;

        public void LoadLevel(LevelName level, System.Action onCompleted = null, System.Action<DiContainer> installBindingsAction = null)
        {
            if (Current == level)
            {
                Observable.EveryUpdate().First().Subscribe(_ =>
                {
                    OptimizedDebug.Log("Level already loaded: " + level.ToSceneId());
                    onCompleted?.Invoke();
                });
                return;
            }

            _sceneBeforeUnloadTrigger.Fire();

            Current = level;

            IsLoading = true;

            SceneContext.BeforeInstallHooks = installBindingsAction;
            SceneManager.LoadSceneAsync(level.ToSceneId()).AsObservable().Subscribe(_ =>
            {
                IsLoading = false;
                OptimizedDebug.Log("Level loaded: " + Current);
                _sceneLoadedTrigger.Fire();
                onCompleted?.Invoke();
            });
        }
    }

    public class SceneBeforeUnloadSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

    public class SceneLoadedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
