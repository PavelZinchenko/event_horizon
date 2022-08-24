using System;
using UnityEngine;
using System.Collections.Generic;
using Combat.Component.Helpers;
using Combat.Factory;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Services.Reources;
using Zenject;

public class PrefabCache : MonoBehaviour
{
    [Inject] private readonly IResourceLocator _resourceLocator;

    public GameObject LoadResourcePrefab(string path, bool noExceptions = false)
    {
        GameObject prefab;
        if (!_prefabs.TryGetValue(path, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                if (!noExceptions) Debug.LogException(new ArgumentException("prefab not found: " + path));
                return null;
            }

            _prefabs[path] = prefab;
        }

        return prefab;
    }

    public GameObject LoadPrefab(string path)
	{
		GameObject prefab;
		if (!_prefabs.TryGetValue(path, out prefab))
		{
			prefab = Resources.Load<GameObject>("Prefabs/" + path);
		    if (prefab == null)
		    {
		        Debug.LogException(new ArgumentException("prefab not found: " + path));
		        return null;
		    }

		    _prefabs[path] = prefab;
		}

		return prefab;
	}

    public GameObject LoadPrefab(PrefabId prefabId)
    {
        var path = prefabId.ToString();
        GameObject prefab;
        if (!_prefabs.TryGetValue(path, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            _prefabs[path] = prefab;
        }

        return prefab;
    }

    public GameObject GetBulletPrefab(BulletPrefab data)
    {
        var id = data != null ? data.Id.Value : 0;
        GameObject prefab;
        if (_bulletPrefabs.TryGetValue(id, out prefab))
            return prefab;

        GameObject commonPrefab;
        if (data == null)
            commonPrefab = LoadPrefab(new PrefabId("AreaOfEffect", PrefabId.Type.Bullet));
        else
            switch (data.Shape)
            {
                case BulletShape.Projectile: commonPrefab = LoadPrefab(new PrefabId("CommonProjectile", PrefabId.Type.Bullet)); break;
                case BulletShape.Rocket: commonPrefab = LoadPrefab(new PrefabId("CommonRocket", PrefabId.Type.Bullet)); break;
                case BulletShape.LaserBeam: commonPrefab = LoadPrefab(new PrefabId("Laser", PrefabId.Type.Bullet)); break;
                case BulletShape.LightningBolt: commonPrefab = LoadPrefab(new PrefabId("Lightning", PrefabId.Type.Bullet)); break;
                case BulletShape.EnergyBeam: commonPrefab = LoadPrefab(new PrefabId("EnergyBeam", PrefabId.Type.Bullet)); break;
                case BulletShape.Spark: commonPrefab = LoadPrefab(new PrefabId("Spark", PrefabId.Type.Bullet)); break;
                case BulletShape.Mine: commonPrefab = LoadPrefab(new PrefabId("Mine", PrefabId.Type.Bullet)); break;
                default: return null;
            }

        prefab = Instantiate(commonPrefab);
        prefab.SetActive(false);
        prefab.transform.parent = transform;
        prefab.GetComponent<IBulletPrefabInitializer>().Initialize(data, _resourceLocator);

        _bulletPrefabs.Add(id, prefab);
        return prefab;
    }

    public GameObject GetEffectPrefab(VisualEffectElement data)
    {
        var id = (int)data.Type + data.Image.Id;
        GameObject prefab;
        if (_effectPrefabs.TryGetValue(id, out prefab))
            return prefab;

        GameObject commonPrefab;
        switch (data.Type)
        {
            case VisualEffectType.Flash: commonPrefab = LoadPrefab(new PrefabId("Flash", PrefabId.Type.Effect)); break;
            case VisualEffectType.FlashAdditive: commonPrefab = LoadPrefab(new PrefabId("FlashAdditive", PrefabId.Type.Effect)); break;
            case VisualEffectType.Shockwave: commonPrefab = LoadPrefab(new PrefabId("Wave", PrefabId.Type.Effect)); break;
            case VisualEffectType.Smoke: commonPrefab = LoadPrefab(new PrefabId("Smoke", PrefabId.Type.Effect)); break;
            case VisualEffectType.SmokeAdditive: commonPrefab = LoadPrefab(new PrefabId("SmokeAdditive", PrefabId.Type.Effect)); break;
            default: return null;
        }

        prefab = Instantiate(commonPrefab);
        prefab.SetActive(false);
        prefab.transform.parent = transform;
        prefab.GetComponent<IEffectPrefabInitializer>().Initialize(data, _resourceLocator);

        _effectPrefabs.Add(id, prefab);
        return prefab;
    }

    private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
    private readonly Dictionary<int, GameObject> _bulletPrefabs = new Dictionary<int, GameObject>();
    private readonly Dictionary<string, GameObject> _effectPrefabs = new Dictionary<string, GameObject>();
}
