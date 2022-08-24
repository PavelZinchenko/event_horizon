using System;
using System.Collections.Generic;
using Services.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

public static class LayoutGroupExtensions
{
	public static int InitializeElements<ViewModelType, ModelType>(
		this LayoutGroup layout,
		IEnumerable<ModelType> data,
		Action<ViewModelType, ModelType> Initializer,
        GameObjectFactory factory)
		where ViewModelType: MonoBehaviour
	{
		int count = 0;
		var enumerator = data.GetEnumerator();
		ViewModelType item = null;
		foreach (Transform transform in layout.transform)
		{
			var viewModel = transform.GetComponent<ViewModelType>();
			if (viewModel == null)
				continue;
			item = viewModel;
			if (enumerator.MoveNext())
			{
				item.gameObject.SetActive(true);
				Initializer(item, enumerator.Current);
				count++;
			}
			else
				item.gameObject.SetActive(false);
		}

	    if (item == null)
	        return count;

		var parent = item.transform.parent;
		var index = item.transform.GetSiblingIndex();

		while (enumerator.MoveNext())
		{
			var newItem = factory.Create(item.gameObject).GetComponent<ViewModelType>();
			var rectTransform = newItem.GetComponent<RectTransform>();
			rectTransform.SetParent(parent);
			rectTransform.localScale = Vector3.one;
			rectTransform.SetSiblingIndex(++index);
			newItem.gameObject.SetActive(true);
			Initializer(newItem, enumerator.Current);
			count++;
		}

		return count;
	}

    public static int InitializeElements<ViewModelType, ModelType>(
        this LayoutGroup layout,
        IEnumerable<ModelType> data,
        Action<ViewModelType, ModelType> Initializer)
        where ViewModelType : MonoBehaviour
    {
        int count = 0;
        var enumerator = data.GetEnumerator();
        ViewModelType item = null;
        foreach (Transform transform in layout.transform)
        {
            var viewModel = transform.GetComponent<ViewModelType>();
            if (viewModel == null)
                continue;
            item = viewModel;
            if (enumerator.MoveNext())
            {
                item.gameObject.SetActive(true);
                Initializer(item, enumerator.Current);
                count++;
            }
            else
                item.gameObject.SetActive(false);
        }

        if (item == null)
            return count;

        var parent = item.transform.parent;
        var index = item.transform.GetSiblingIndex();

        while (enumerator.MoveNext())
        {
            var newItem = (ViewModelType)GameObject.Instantiate(item);
            var rectTransform = newItem.GetComponent<RectTransform>();
            rectTransform.SetParent(parent);
            rectTransform.localScale = Vector3.one;
            rectTransform.SetSiblingIndex(++index);
            newItem.gameObject.SetActive(true);
            Initializer(newItem, enumerator.Current);
            count++;
        }

        return count;
    }
}
