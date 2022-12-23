using System;
using UnityEngine;
using System.Collections.Generic;
using GameServices.LevelManager;
using UnityEngine.Pool;
using Utils;
using Zenject;
using Object = UnityEngine.Object;

namespace Services.ObjectPool
{
    public class GameObjectPool : MonoBehaviour, IObjectPool
    {
        [Inject]
        private void Initialize(GameObjectFactory factory, SceneBeforeUnloadSignal sceneBeforeUnloadSignal)
        {
            _factory = factory;
            _sceneBeforeUnloadSignal = sceneBeforeUnloadSignal;
            _sceneBeforeUnloadSignal.Event += OnSceneBeforeUnload;
            WithInjections = new Pool(prefab => () => _factory.Create(prefab));
            WithoutInjections = new Pool(prefab => () => Instantiate(prefab));
        }

        private SceneBeforeUnloadSignal _sceneBeforeUnloadSignal;
        private GameObjectFactory _factory;

        private void OnSceneBeforeUnload()
        {
            WithInjections.OnSceneBeforeUnload();
            WithoutInjections.OnSceneBeforeUnload();
        }

        private void OnDestroy()
        {
            WithInjections.Enabled = false;
            WithoutInjections.Enabled = false;
        }

        private Pool WithInjections;
        private Pool WithoutInjections;

        private class Pool : IObjectPool
        {
            private readonly Func<GameObject, Func<GameObject>> _createFunc;

            public Pool(Func<GameObject, Func<GameObject>> createFunc)
            {
                _createFunc = createFunc;
            }

            public bool Enabled = true;

            public GameObject GetObject(GameObject prefab)
            {
                if (!Enabled)
                    return Instantiate(prefab);

                GameObject gameObject = null;
                if (!_pools.TryGetValue(prefab, out var pool))
                {
                    pool = _pools[prefab] = CreatePool(prefab, 8);
                }

                return pool.Get();
            }

            public void PreloadObjects(GameObject prefab, int count)
            {
                if (prefab == null)
                    return;

                if (!_pools.ContainsKey(prefab))
                {
                    _pools[prefab] = CreatePool(prefab, count);
                }
            }

            public IObjectPool NoInjections => this;

            public void ReleaseObject(GameObject gameObject)
            {
                if (!Enabled)
                {
                    Destroy(gameObject);
                    return;
                }

                if (!gameObject)
                    return;

                GameObject prefab;
                if (!_objectPrefabs.TryGetValue(gameObject, out prefab))
                {
                    Destroy(gameObject);
                    return;
                }

                if (!_pools.TryGetValue(prefab, out var pool))
                {
                    pool = _pools[prefab] = CreatePool(prefab, 8);
                }

                pool.Release(gameObject);
            }

            private IObjectPool<GameObject> CreatePool(GameObject prefab, int preload)
            {
                return new ObjectPool<GameObject>(
                    createFunc: _createFunc(prefab),
                    actionOnGet: go => { _objectPrefabs[go] = prefab; },
                    actionOnRelease: go =>
                    {
                        go.SetActive(false);
                        var transform = go.transform;
                        if (!ReferenceEquals(transform.parent, null)) transform.parent = null;
                        _objectPrefabs.Remove(go);
                    },
                    actionOnDestroy: go =>
                    {
                        _objectPrefabs.Remove(go);
                        Destroy(go);
                    },
                    collectionCheck: false,
                    defaultCapacity: preload
                );
            }

            public void OnSceneBeforeUnload()
            {
                try
                {
                    foreach (var pool in _pools.Values)
                    {
                        pool.Clear();
                    }

                    _pools.Clear();
                    _objectPrefabs.Clear();
                }
                catch (Exception e)
                {
                    OptimizedDebug.LogException(e);
                }
            }

            private readonly Dictionary<GameObject, GameObject> _objectPrefabs =
                new Dictionary<GameObject, GameObject>();

            private readonly Dictionary<Object, IObjectPool<GameObject>> _pools =
                new Dictionary<Object, IObjectPool<GameObject>>();
        }

        public GameObject GetObject(GameObject prefab)
        {
            return WithInjections.GetObject(prefab);
        }

        public void ReleaseObject(GameObject gameObject)
        {
            WithInjections.ReleaseObject(gameObject);
        }

        public void PreloadObjects(GameObject prefab, int count)
        {
            WithInjections.PreloadObjects(prefab, count);
        }

        public IObjectPool NoInjections => WithoutInjections;
    }

    public class GameObjectFactory : IFactory<GameObject, GameObject>
    {
        [Inject] private readonly DiContainer _container;

        public GameObject Create(GameObject param)
        {
            var gameObject = _container.InstantiatePrefab(param);

            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform != null)
                rectTransform.SetParent(null, false);
            else
                gameObject.transform.parent = null;

            return gameObject;
        }
    }
}
