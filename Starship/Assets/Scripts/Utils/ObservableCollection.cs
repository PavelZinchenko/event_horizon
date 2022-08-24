using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Utils
{
    public interface IItemCollection<T> : IList<T>
    {
        void Assign(IEnumerable<T> items);
        IItemCollection<T> AsReadOnly();
    }

    public class ObservableCollection<T> : IItemCollection<T>
    {
        public ObservableCollection(IEnumerable<T> items = null, bool readOnly = false)
        {
            _list = items != null ? new List<T>(items) : new List<T>();
            _readOnly = readOnly;
        }

        public event Action DataChangedEvent;
        public event Action<T> ItemAddedEvent;
        public event Action<T> ItemRemovedEvent;
        public event Action EntireCollectionChangedEvent;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (_readOnly)
                throw new InvalidOperationException();

            _list.Add(item);
            InvokeItemAddedEvent(item);
        }

        public void Clear()
        {
            if (_readOnly)
                throw new InvalidOperationException();

            _list.Clear();
            InvokeEntireCollectionChangedEvent();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_readOnly)
                throw new InvalidOperationException();

            if (!_list.Remove(item))
                return false;

            InvokeItemRemovedEvent(item);
            return true;
        }

        public int Count { get { return _list.Count; } }
        public bool IsReadOnly { get { return _readOnly; } }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (_readOnly)
                throw new InvalidOperationException();

            _list.Insert(index, item);
            InvokeItemAddedEvent(item);
        }

        public void RemoveAt(int index)
        {
            if (_readOnly)
                throw new InvalidOperationException();

            var item = _list[index];
            _list.RemoveAt(index);

            InvokeItemRemovedEvent(item);
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                if (_readOnly)
                    throw new InvalidOperationException();

                var oldValue = _list[index];
                _list[index] = value;

                InvokeItemRemovedEvent(oldValue);
                InvokeItemAddedEvent(value);
            }
        }

        public void Assign(IEnumerable<T> items)
        {
            Assert.IsNotNull(items);

            if (_readOnly)
                throw new InvalidOperationException();

            _list.Clear();
            _list.AddRange(items);

            InvokeEntireCollectionChangedEvent();
        }

        public IItemCollection<T> AsReadOnly()
        {
            if (_readOnly)
                return this;

            return _readOnlyCollection ?? (_readOnlyCollection = new ObservableCollection<T>(this));
        }

        private void InvokeItemAddedEvent(T item)
        {
            if (ItemAddedEvent != null)
                ItemAddedEvent.Invoke(item);
            if (_readOnlyCollection != null && _readOnlyCollection.ItemAddedEvent != null)
                _readOnlyCollection.InvokeItemAddedEvent(item);
            if (DataChangedEvent != null)
                DataChangedEvent.Invoke();
        }

        private void InvokeItemRemovedEvent(T item)
        {
            if (ItemRemovedEvent != null)
                ItemRemovedEvent.Invoke(item);
            if (_readOnlyCollection != null && _readOnlyCollection.ItemRemovedEvent != null)
                _readOnlyCollection.InvokeItemRemovedEvent(item);
            if (DataChangedEvent != null)
                DataChangedEvent.Invoke();
        }

        private void InvokeEntireCollectionChangedEvent()
        {
            if (EntireCollectionChangedEvent != null)
                EntireCollectionChangedEvent.Invoke();
            if (_readOnlyCollection != null && _readOnlyCollection.EntireCollectionChangedEvent != null)
                _readOnlyCollection.InvokeEntireCollectionChangedEvent();
            if (DataChangedEvent != null)
                DataChangedEvent.Invoke();
        }

        private ObservableCollection(ObservableCollection<T> other)
        {
            _list = other._list;
            _readOnly = true;
        }

        private readonly bool _readOnly;
        private readonly List<T> _list;
        private ObservableCollection<T> _readOnlyCollection;
    }
}
