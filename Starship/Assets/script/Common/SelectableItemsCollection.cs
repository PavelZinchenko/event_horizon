using System.Collections.Generic;

public class SelectableItemsCollection<T>
	where T: class
{
	public SelectableItemsCollection(int size)
	{
		_items = new HashSet<T>();
		_selected = new T[size];
	}

	public void Add(T item)
	{
		_items.Add(item);
	}

	public T this[int index]
	{
		get { return _selected[index]; }
		set
		{
			if (_items.Contains(value))
				_selected[index] = value; 
			else
				throw new System.ArgumentException();
		}
	}

	public Dictionary<T, int> Group(IEqualityComparer<T> comparer)
	{
		var dictionary = new Dictionary<T, int>(comparer);
		foreach (var item in _items)
		{
			int count;
			dictionary[item] = dictionary.TryGetValue(item, out count) ? count + 1 : 1;
		}
		foreach (var item in _selected)
		{
			if (item != null)
				dictionary[item] = dictionary[item] - 1;
		}
		return dictionary;
	}

	public IEnumerable<T> Items { get { return _items; } }

	private T[] _selected;
	private HashSet<T> _items;
}
