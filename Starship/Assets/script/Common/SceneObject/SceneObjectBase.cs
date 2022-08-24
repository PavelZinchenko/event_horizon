/*using UnityEngine;

namespace Common
{
	public abstract class SceneObjectBase<T> : ISceneObject
		where T : class, ISceneObject
	{
		public static T Instance { get { return _instance; } }

		public virtual void Initialize() {}
		public virtual void OnUpdate(float elapsedTime) {}
		public virtual void OnPause() {}
		public virtual void OnResume() {}

		public virtual void OnDestroy()
		{
			_instance = null;
		}

		protected void InitInstance(T instance)
		{
			if (_instance != null)
				throw new System.InvalidOperationException("attempting to create second instance");
			_instance = instance;
		}

		private static T _instance;
	}
}
*/