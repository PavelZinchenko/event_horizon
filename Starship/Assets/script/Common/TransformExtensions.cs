using System;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static IEnumerable<T> Children<T>(this Transform transform) 
        where T : Component
    {
        foreach (Transform child in transform)
        {
            var item = child.GetComponent<T>();
            if (item == null)
                continue;

            yield return item;
        }
    }

    public static IEnumerable<T> CreateChildren<T>(this Transform transform)
        where T : Component
    {
        T prefab = null;

        foreach (Transform child in transform)
        {
            var item = child.GetComponent<T>();
            if (item == null)
                continue;

            prefab = item;

            item.gameObject.SetActive(true);
            yield return item;
        }

        if (prefab == null)
            throw new InvalidOperationException();

        for (int i = 0; i < 500; ++i)
        {
            var child = GameObject.Instantiate(prefab.gameObject);
            child.gameObject.SetActive(true);

            yield return child.GetComponent<T>();
        }

        throw new InvalidOperationException();
    }

    public static void DeactivateChildren<T>(this Transform transform)
        where T : Component
    {
        foreach (var child in transform.Children<T>())
            child.gameObject.SetActive(false);
    }
}
