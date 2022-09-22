using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Services.Unity
{
    public class CoroutineManager : MonoBehaviour, ICoroutineManager
    {
        public void StartActionOnNextUpdate(Action<string> action, string param)
        {
            _stringActions.Enqueue(new ActionWithParam<string> { Action = action, Param = param });
        }

        public void StartActionOnNextUpdate(Action<string, string> action, string param1, string param2)
        {
            _doubleStringActions.Enqueue(new ActionWithParam<string, string> { Action = action, Param1 = param1, Param2 = param2 } );
        }

        private void Update()
        {
            try
            {
                InvokeActions(_stringActions);
                InvokeActions(_doubleStringActions);
            }
            catch (Exception e)
            {
                OptimizedDebug.LogException(e);
            }
        }

        private void InvokeActions<T>(Queue<ActionWithParam<T>> actions)
        {
            while (actions.Count > 0)
            {
                var action = actions.Dequeue();
                action.Invoke();
            }
        }

        private void InvokeActions<T, U>(Queue<ActionWithParam<T, U>> actions)
        {
            while (actions.Count > 0)
            {
                var action = actions.Dequeue();
                action.Invoke();
            }
        }

        private readonly Queue<ActionWithParam<string>> _stringActions = new Queue<ActionWithParam<string>>();
        private readonly Queue<ActionWithParam<string, string>> _doubleStringActions = new Queue<ActionWithParam<string, string>>();

        private struct ActionWithParam<T>
        {
            public Action<T> Action;
            public T Param;

            public void Invoke()
            {
                Action.Invoke(Param);
            }
        }

        private struct ActionWithParam<T, U>
        {
            public Action<T, U> Action;
            public T Param1;
            public U Param2;

            public void Invoke()
            {
                Action.Invoke(Param1, Param2);
            }
        }
    }
}
