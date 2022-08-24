using UnityEngine;

namespace Services.ObjectPool
{
	public interface IObjectPool
	{
		GameObject GetObject(GameObject prefab, bool injectDependencies = true);
		void ReleaseObject(GameObject gameObject);
		void PreloadObjects(GameObject prefab, int count, bool injectDependencies = true);
	}
}
