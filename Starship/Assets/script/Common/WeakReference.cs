using System;

public class WeakReference<T>
{
	public WeakReference(T target)
	{
		_weakReference = new WeakReference(target);
	}

	public bool IsAlive
	{
		get { return _weakReference.IsAlive; }
	}

	public T Target
	{
		get	{ return (T)_weakReference.Target; }
	}

	private readonly WeakReference _weakReference;
}
