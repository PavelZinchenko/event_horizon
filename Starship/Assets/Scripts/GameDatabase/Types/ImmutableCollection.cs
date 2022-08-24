using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameDatabase.Model
{
    public struct ImmutableCollection<T> : IReadOnlyCollection<T>
    {
        public ImmutableCollection(IEnumerable<T> items)
        {
            _items = items != null ? new List<T>(items) : null;
        }

        public static ImmutableCollection<T> operator +(ImmutableCollection<T> source, T item)
        {
            var collection = new ImmutableCollection<T>(source);
            collection._items.Add(item);
            return collection;
        }

        public static ImmutableCollection<T> operator +(T item, ImmutableCollection<T> source)
        {
            return new ImmutableCollection<T>(source.Prepend(item));
        }

        public static ImmutableCollection<T> operator +(ImmutableCollection<T> source, IEnumerable<T> items)
        {
            var collection = new ImmutableCollection<T>(source);
            collection._items.AddRange(items);
            return collection;
        }

        public static ImmutableCollection<T> operator +(ImmutableCollection<T> first, ImmutableCollection<T> second)
        {
            if (first.Count == 0) return second;
            if (second.Count == 0) return first;

            var collection = new ImmutableCollection<T>(first);
            collection._items.AddRange(second);
            return collection;
        }

        public static ImmutableCollection<T> operator -(ImmutableCollection<T> source, T item)
        {
            var index = source.IndexOf(item);
            if (index < 0) return source;

            var count = source.Count;
            var collection = new ImmutableCollection<T>(Enumerable.Empty<T>());
            for (var i = 0; i < count; ++i)
            {
                if (i == index) continue;
                collection._items.Add(source._items[i]);
            }

            return collection;
        }

        public int IndexOf(T item)
        {
            return _items?.IndexOf(item) ?? -1;
        }

        public T this[int index]
        {
            get
            {
                if (_items == null || index < 0 || index >= _items.Count)
                    throw new IndexOutOfRangeException("ImmutableCollection: index out of range - " + index + " (" + Count + " elements)");

                return _items[index];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _items?.Count ?? 0;

        private readonly List<T> _items;
    }
}
