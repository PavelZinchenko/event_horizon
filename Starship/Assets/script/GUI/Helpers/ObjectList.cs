using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class ObjectList : MonoBehaviour
{
    [SerializeField] GameObject[] _children = {};

    public GameObject[] Children { get { return _children; } }

#if UNITY_EDITOR
    protected virtual void OnTransformChildrenChanged()
    {
        if (Application.isPlaying)
            return;

        var children = new HashSet<GameObject>();
        foreach (Transform child in transform)
            children.Add(child.gameObject);

        foreach (var item in _children)
            children.Remove(item);

        var size = children.Count;
        if (size == 0)
            return;

        var enumerator = children.GetEnumerator();
        enumerator.MoveNext();

        for (var i = 0; i < _children.Length; ++i)
        {
            if (_children[i])
                continue;

            _children[i] = enumerator.Current;
            size--;

            if (!enumerator.MoveNext())
                break;
        }

        if (size == 0)
            return;

        var result = new GameObject[_children.Length + size];
        _children.CopyTo(result, 0);

        for (var i = _children.Length; i < result.Length; ++i)
        {
            result[i] = enumerator.Current;
            enumerator.MoveNext();
        }

        _children = result;
    }
#endif
}
