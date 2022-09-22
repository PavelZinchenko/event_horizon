using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using GameServices.LevelManager;
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
	    }

        private SceneBeforeUnloadSignal _sceneBeforeUnloadSignal;
        private GameObjectFactory _factory;

        public GameObject GetObject(GameObject prefab, bool injectDependencies = true)
        {
            if (!this)
                return GameObject.Instantiate(prefab);

            GameObject gameObject = null;
			HashSet<GameObject> objects;
			if (_unusedObjectsCache.TryGetValue(prefab, out objects))
			{
			    var needCleanup = false;
			    foreach (var item in objects)
			    {
			        if (item == null)
			        {
			            needCleanup = true;
                        continue;
			        }

			        gameObject = item;
                    objects.Remove(item);
                    break;
			    }

			    if (needCleanup)
			        objects.RemoveWhere(item => item == null);
			}

            if (gameObject == null)
                gameObject = injectDependencies ? (GameObject)_factory.Create(prefab) : (GameObject)Instantiate(prefab);
			if (gameObject == null)
				throw new System.ArgumentException();

			_objectPrefabs[gameObject] = prefab;
            gameObject.transform.parent = null;
			return gameObject;
		}

		public void PreloadObjects(GameObject prefab, int count, bool injectDependencies = true)
		{
            if (prefab == null)
                return;

			HashSet<GameObject> objects;
			if (!_unusedObjectsCache.TryGetValue(prefab, out objects)) 
			{
				objects = new HashSet<GameObject>();
				_unusedObjectsCache.Add(prefab, objects);
			}

			var available = objects.Count(item => item != null);

			for (var i = available; i < count; ++i)
			{
			    var gameObject = injectDependencies ? (GameObject)_factory.Create(prefab) : (GameObject)Instantiate(prefab);
				if (gameObject == null)
					throw new System.ArgumentException();
				
				_objectPrefabs[gameObject] = prefab;
				gameObject.transform.parent = transform;
				gameObject.SetActive(false);
				objects.Add(gameObject);
			}
		}

		public void ReleaseObject(GameObject gameObject)
		{
		    if (!this)
		    {
		        GameObject.Destroy(gameObject);
                return;
		    }

            if (!gameObject)
                return;

			gameObject.SetActive(false);
			gameObject.transform.parent = transform;

			GameObject prefab;
			if (!_objectPrefabs.TryGetValue(gameObject, out prefab))
			{
				GameObject.Destroy(gameObject);
				return;
			}

			HashSet<GameObject> objects;
			if (!_unusedObjectsCache.TryGetValue(prefab, out objects))
			{
				objects = new HashSet<GameObject>();
				_unusedObjectsCache.Add(prefab, objects);
			}

			objects.Add(gameObject);
		}

        private void OnSceneBeforeUnload()
        {
            try
            {
                _objectPrefabs.Clear();

                foreach (var items in _unusedObjectsCache.Values)
                {
                    foreach (var item in items)
                    {
                        item.SendMessage("OnDestroy", SendMessageOptions.DontRequireReceiver);
                        Destroy(item);
                    }

                    items.Clear();
                }

                _unusedObjectsCache.Clear();
            }
            catch (Exception e)
            {
                OptimizedDebug.LogException(e);
            }
        }

        private void RemoveUnusedPrefabs()
		{
			foreach (var key in _objectPrefabs.Keys.Where(item => !item).ToList())
			{
				_objectPrefabs.Remove(key);
			}
		}

		private void Update()
		{
			var time = Time.realtimeSinceStartup;
			if (time - _lastUpdateTime > 10.0f)
			{
				_lastUpdateTime = time;
                RemoveUnusedPrefabs();
			}
		}

		private float _lastUpdateTime;
		private readonly Dictionary<GameObject, GameObject> _objectPrefabs = new Dictionary<GameObject, GameObject>();
		private readonly Dictionary<Object, HashSet<GameObject>> _unusedObjectsCache = new Dictionary<Object, HashSet<GameObject>>();
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
