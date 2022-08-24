using System;

public class Lazy<T>
{
	public Lazy(Func<T> initializer)
	{
		_initializer = initializer;
	}

    public bool HasValue { get; private set; }

    public static implicit operator T(Lazy<T> lazy)
    {
        return lazy.Value;
    }

    public T Value
	{
		get
		{
			if (!HasValue)
			{
				_value = _initializer();
				HasValue = true;
			}

			return _value;
		}
	}

	public void Reset()
	{
		_value = default(T);
		HasValue = false;
	}

    private T _value;
	private readonly Func<T> _initializer;
}