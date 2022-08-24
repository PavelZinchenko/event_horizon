using UnityEngine;

namespace Common
{
	public abstract class SceneObjectHolder : MonoBehaviour
	{
		protected abstract ISceneObject CreateInstance();

		private void Awake()
		{
			_instance = CreateInstance();
		}

		private void Start()
		{
			_instance.Initialize();
		}

		private void Update()
		{
			_instance.OnUpdate(Time.deltaTime);
		}

		private void FixedUpdate()
		{
			_instance.OnFixedUpdate();
		}

		private void OnApplicationFocus(bool focusStatus) 
		{
			if (focusStatus)
				_instance.OnResume();
			else
				_instance.OnPause();
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused)
				_instance.OnPause();
			else
				_instance.OnResume();
		}

		private void OnDestroy()
		{
			_instance.OnDestroy();
		}

		private ISceneObject _instance;
	}
}
