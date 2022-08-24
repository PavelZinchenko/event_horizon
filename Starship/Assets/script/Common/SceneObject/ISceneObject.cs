using UnityEngine;

namespace Common
{
	public interface ISceneObject
	{
		void Initialize();
		void OnUpdate(float elapsedTime);
		void OnFixedUpdate();
		void OnPause();
		void OnResume();
		void OnDestroy();
	}
}
