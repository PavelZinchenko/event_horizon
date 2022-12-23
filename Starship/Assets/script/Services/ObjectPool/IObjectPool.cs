using UnityEngine;

namespace Services.ObjectPool
{
	public interface IObjectPool
	{
		GameObject GetObject(GameObject prefab);
		void ReleaseObject(GameObject gameObject);
		void PreloadObjects(GameObject prefab, int count);
		IObjectPool NoInjections { get; }
	}
}
