using System;
using Zenject;
using SmartWeakEvent;

namespace Utils
{
    public class SmartWeakSignal : ISignal
    {
        public event Action Event
        {
            add { _event.Add(value); }
            remove { _event.Remove(value); }
        }

        private readonly SmartWeakEvent<Action> _event = new SmartWeakEvent<Action>();

        public abstract class TriggerBase : ITrigger
        {
            [Inject] private readonly SmartWeakSignal _signal = null;

            public event Action Event
            {
                add { _signal._event.Add(value); }
                remove { _signal._event.Remove(value); }
            }

            public void Fire()
            {
                _signal._event.Raise();
            }
        }
    }

    public class SmartWeakSignal<TParam1> : ISignal
    {
        public event Action<TParam1> Event
        {
            add { _event.Add(value); }
            remove { _event.Remove(value); }
        }

        private readonly SmartWeakEvent<Action<TParam1>> _event = new SmartWeakEvent<Action<TParam1>>();

        public abstract class TriggerBase : ITrigger
        {
            [Inject] private readonly SmartWeakSignal<TParam1> _signal = null;

            public event Action<TParam1> Event
            {
                add { _signal._event.Add(value); }
                remove { _signal._event.Remove(value); }
            }

            public void Fire(TParam1 param1)
            {
                _signal._event.Raise(param1);
            }
        }
    }

    public class SmartWeakSignal<TParam1, TParam2> : ISignal
    {
        public event Action<TParam1, TParam2> Event
        {
            add { _event.Add(value); }
            remove { _event.Remove(value); }
        }

        private readonly SmartWeakEvent<Action<TParam1, TParam2>> _event = new SmartWeakEvent<Action<TParam1, TParam2>>();

        public abstract class TriggerBase : ITrigger
        {
            [Inject]
            private readonly SmartWeakSignal<TParam1, TParam2> _signal = null;

            public event Action<TParam1, TParam2> Event
            {
                add { _signal._event.Add(value); }
                remove { _signal._event.Remove(value); }
            }

            public void Fire(TParam1 param1, TParam2 param2)
            {
                _signal._event.Raise(param1, param2);
            }
        }
    }

    public class SmartWeakSignal<TParam1, TParam2, TParam3> : ISignal
    {
        public event Action<TParam1, TParam2, TParam3> Event
        {
            add { _event.Add(value); }
            remove { _event.Remove(value); }
        }

        private readonly SmartWeakEvent<Action<TParam1, TParam2, TParam3>> _event = new SmartWeakEvent<Action<TParam1, TParam2, TParam3>>();

        public abstract class TriggerBase : ITrigger
        {
            [Inject]
            private readonly SmartWeakSignal<TParam1, TParam2, TParam3> _signal = null;

            public event Action<TParam1, TParam2, TParam3> Event
            {
                add { _signal._event.Add(value); }
                remove { _signal._event.Remove(value); }
            }

            public void Fire(TParam1 param1, TParam2 param2, TParam3 param3)
            {
                _signal._event.Raise(param1, param2, param3);
            }
        }
    }
}
