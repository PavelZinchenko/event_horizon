using System;
using System.Collections.Generic;
using GameServices.LevelManager;
using Zenject;

namespace Services.Messenger
{
    public class Messenger : IMessenger
    {
        [Inject]
        public Messenger(SceneBeforeUnloadSignal sceneBeforeUnloadSignal)
        {
            _sceneBeforeUnloadSignal = sceneBeforeUnloadSignal;
            _sceneBeforeUnloadSignal.Event += Cleanup;
        }

        public void AddListener(EventType eventType, Callback handler)
        {
            _listeners[eventType] = (Callback)GetListenerForAdding(eventType, handler) + handler;
        }

        public void AddListener<T>(EventType eventType, Callback<T> handler)
        {
            _listeners[eventType] = (Callback<T>)GetListenerForAdding(eventType, handler) + handler;
        }

        public void AddListener<T, U>(EventType eventType, Callback<T, U> handler)
        {
            _listeners[eventType] = (Callback<T, U>)GetListenerForAdding(eventType, handler) + handler;
        }

        public void AddListener<T, U, V>(EventType eventType, Callback<T, U, V> handler)
        {
            _listeners[eventType] = (Callback<T, U, V>)GetListenerForAdding(eventType, handler) + handler;
        }

        public void RemoveListener(EventType eventType, Callback handler)
        {
            _listeners[eventType] = (Callback)GetListenerForRemoving(eventType, handler) - handler;
        }

        public void RemoveListener<T>(EventType eventType, Callback<T> handler)
        {
            _listeners[eventType] = (Callback<T>)GetListenerForRemoving(eventType, handler) - handler;
        }

        public void RemoveListener<T, U>(EventType eventType, Callback<T, U> handler)
        {
            _listeners[eventType] = (Callback<T, U>)GetListenerForRemoving(eventType, handler) - handler;
        }

        public void RemoveListener<T, U, V>(EventType eventType, Callback<T, U, V> handler)
        {
            _listeners[eventType] = (Callback<T, U, V>)GetListenerForRemoving(eventType, handler) - handler;
        }

        public void Broadcast(EventType eventType)
        {
            Delegate d;
            if (!_listeners.TryGetValue(eventType, out d))
                return;

            Callback callback = d as Callback;
            if (callback != null)
            {
                callback();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Broadcast<T>(EventType eventType, T arg1)
        {
            //OptimizedDebug.Log("Broadcast - " + eventType);

            Delegate d;
            if (!_listeners.TryGetValue(eventType, out d))
                return;

            Callback<T> callback = d as Callback<T>;
            if (callback != null)
            {
                callback(arg1);
            }
            else
            {
                throw new ArgumentException();
            }

            //OptimizedDebug.Log("Broadcast - done - " + eventType);
        }

        public void Broadcast<T, U>(EventType eventType, T arg1, U arg2)
        {
            //OptimizedDebug.Log("Broadcast - " + eventType);

            Delegate d;
            if (!_listeners.TryGetValue(eventType, out d))
                return;

            Callback<T, U> callback = d as Callback<T, U>;
            if (callback != null)
            {
                callback(arg1, arg2);
            }
            else
            {
                throw new ArgumentException();
            }

            //OptimizedDebug.Log("Broadcast - done - " + eventType);
        }

        public void Broadcast<T, U, V>(EventType eventType, T arg1, U arg2, V arg3)
        {
            //OptimizedDebug.Log("Broadcast - " + eventType);

            Delegate d;
            if (!_listeners.TryGetValue(eventType, out d))
                return;

            Callback<T, U, V> callback = d as Callback<T, U, V>;
            if (callback != null)
            {
                callback(arg1, arg2, arg3);
            }
            else
            {
                throw new ArgumentException();
            }

            //OptimizedDebug.Log("Broadcast - done - " + eventType);
        }

        private void Cleanup()
        {
            _listeners.Clear();
        }

        private Delegate GetListenerForAdding(EventType eventType, Delegate listenerBeingAdded)
        {
            if (!_listeners.ContainsKey(eventType))
            {
                _listeners.Add(eventType, null);
            }

            var d = _listeners[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ArgumentException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }

            return d;
        }

        private Delegate GetListenerForRemoving(EventType eventType, Delegate listenerBeingRemoved)
        {
            Delegate d;
            if (_listeners.TryGetValue(eventType, out d))
            {
                if (d == null)
                {
                    throw new ArgumentException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ArgumentException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
            }

            return d;
        }

        private readonly SceneBeforeUnloadSignal _sceneBeforeUnloadSignal;
        private readonly Dictionary<EventType, Delegate> _listeners = new Dictionary<EventType, Delegate>();
    }
}
