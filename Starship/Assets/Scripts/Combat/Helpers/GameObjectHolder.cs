using System.Collections.Generic;
using Combat.Component.Collider;
using Services.ObjectPool;
using UnityEngine;

namespace Combat.Helpers
{
    public class GameObjectHolder : System.IDisposable
    {
        public GameObjectHolder(GameObject prefab, IObjectPool objectPool, bool injectDependencies = true)
        {
            if (!injectDependencies) objectPool = objectPool.NoInjections;
            _objectPool = objectPool;
            _gameObject = objectPool.GetObject(prefab);
        }

        public bool IsActive
        {
            get { return _gameObject.activeSelf; }
            set { _gameObject.SetActive(value); }
        }

        public Layer Layer
        {
            get { return (Layer)_gameObject.layer; }
            set { _gameObject.layer = (int)value; }
        }

        public string Name
        {
            get { return _gameObject.name; }
            set { _gameObject.name = value; }
        }

        public T GetComponent<T>(bool recursive = false)
        {
            return recursive ? _gameObject.GetComponentInChildren<T>() : _gameObject.GetComponent<T>();
        }

        public T AddComponent<T>() where T : UnityEngine.Component
        {
            var component = _gameObject.AddComponent<T>();
            _components.Add(component);
            return component;
        }

        public Transform Transform
        {
            get { return _gameObject.transform; }
        }

        public void Dispose()
        {
            foreach (var item in _components)
                Object.Destroy(item);
            _components.Clear();
            _objectPool.ReleaseObject(_gameObject);
        }

        private readonly GameObject _gameObject;
        private readonly IObjectPool _objectPool;
        private readonly IList<UnityEngine.Component> _components = new List<UnityEngine.Component>();
    }
}
