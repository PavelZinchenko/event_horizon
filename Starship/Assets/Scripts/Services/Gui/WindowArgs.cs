using System;

namespace Services.Gui
{
	public class WindowArgs
	{
		public WindowArgs(params object[] args)
		{
			_args = args;
		}

        public T Get<T>(int index = 0)
		{
			if (_args == null || index >= _args.Length)
				throw new ArgumentOutOfRangeException();

			T value = (T)_args[index];

			return value;
		}

        public bool TryGet<T>(int index, out T result)
        {
            result = default(T);
            if (_args == null || index >= _args.Length)
                return false;

            var value = _args[index];
            if (!(value is T))
                return false;

            result = (T)value;
            return true;
        }

        public int Count { get { return _args.Length; } }

		private readonly object[] _args;
	}
}

