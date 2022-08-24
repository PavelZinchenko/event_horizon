using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameDatabase.Model
{
    public struct ImmutableSet<T> : IReadOnlyCollection<T>
    {
        public ImmutableSet(IEnumerable<T> items)
        {
            _items = items != null ? new HashSet<T>(items) : null;
        }

        public static ImmutableSet<T> operator +(ImmutableSet<T> source, T item)
        {
            if (source.Contains(item)) return source;
            var set = new ImmutableSet<T>(source);
            set._items.Add(item);
            return set;
        }

        public static ImmutableSet<T> operator +(T item, ImmutableSet<T> source)
        {
            return source + item;
        }

        public static ImmutableSet<T> operator +(ImmutableSet<T> source, IEnumerable<T> items)
        {
            var set = new ImmutableSet<T>(source);
            set._items.UnionWith(items);
            return set;
        }

        public static ImmutableSet<T> operator -(ImmutableSet<T> source, T item)
        {
            if (!source.Contains(item)) return source;
            var set = new ImmutableSet<T>(source);
            set._items.Remove(item);
            return set;
        }

        public static ImmutableSet<T> operator -(ImmutableSet<T> source, IEnumerable<T> items)
        {
            var set = new ImmutableSet<T>(source);
            set._items.ExceptWith(items);
            return set;
        }

        public bool Contains(T item)
        {
            return _items?.Contains(item) ?? false;
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
        public bool IsEmpty => _items == null || _items.Count == 0;

        private readonly HashSet<T> _items;
    }
}
