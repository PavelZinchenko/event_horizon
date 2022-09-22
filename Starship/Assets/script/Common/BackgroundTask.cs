using System;
using System.Threading;
using Utils;

public abstract class BackgroundTask
{
	protected abstract bool DoWork();
	protected abstract void OnIdle();
	
	public void StartTask(object owner = null)
	{
		if (!_started)
		{
			_ownerReference = owner != null ? new WeakReference<object>(owner) : null;
			
			var thread = new Thread(ThreadFunc);
			while (thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
			{
				OptimizedDebug.Log("Invalid thread id (" + thread.ManagedThreadId + ")");
				thread = new Thread(ThreadFunc);
			}
			
			thread.Priority = ThreadPriority.Normal;
			thread.Start(this);
			_started = true;
		}
	}

	public void StopTask() { _cancelled = true; }
	public bool Paused { set { _paused = value; } }
	
	private void ThreadFunc(object data)
	{
		var context = (BackgroundTask)data;

	    OptimizedDebug.Log("background task started: " + GetType() + " (" + Thread.CurrentThread.ManagedThreadId + ")");
		
		while (!context._cancelled && (context._ownerReference == null || context._ownerReference.IsAlive))
		{
			while (context._paused)
			{
				Thread.Sleep(100);
			}

			try
			{
				if (!DoWork())
				{
					OnIdle();
				}
			}
			catch (Exception e)
			{
                OptimizedDebug.LogException(e);
			}
		}

	    OptimizedDebug.Log("background task finished: " + GetType());
	}
	
	private WeakReference<object> _ownerReference;
	private bool _started = false;
	private bool _paused = false;
	private bool _cancelled = false;
}
