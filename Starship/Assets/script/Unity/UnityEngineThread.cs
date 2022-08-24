using System;
using System.Collections.Generic;
using System.Threading;
using GameServices.LevelManager;
using Zenject;

public class UnityEngineThread : UnityEngine.MonoBehaviour
{
    [Inject] private readonly ILevelLoader _levelLoader;

	public UnityEngineThread()
	{
		_self = new WeakReference<UnityEngineThread>(this);
	}
	
	public static void Execute(Action action)
	{
	    if (_self == null)
	    {
	        UnityEngine.Debug.Log("UnityEngineThread not yet initialized");
            action.Invoke();
            return;
	    }

		var target = _self.Target;
	    if (!ReferenceEquals(target, null))
	        target.Enqueue(action);
	    else
	        UnityEngine.Debug.Log("UnityEngineThread.Execute: object not initialized");
	}

	private void Update()
	{
        if (_levelLoader.IsLoading)
			return;

		Action action;
		while ((action = Dequeue()) != null)
		{
            //UnityEngine.Debug.Log("UnityEngineThread: ExecuteAction");
            action();
            //UnityEngine.Debug.Log("UnityEngineThread: ExecuteAction - done");
        }
    }
	
	private void Enqueue(Action action)
	{
        //UnityEngine.Debug.Log("UnityEngineThread: Enqueue");

        if (!Monitor.TryEnter(_lockObject, TimeSpan.FromSeconds(1)))
            UnityEngine.Debug.LogException(new Exception("Deadlock"));

        try
        {
            _objects.Enqueue(action);
            //UnityEngine.Debug.Log("UnityEngineThread: Enqueue - " + _objects.Count);
        }
        finally
        {
            Monitor.Exit(_lockObject);
        }
    }
	
	private Action Dequeue()
	{
        if (!Monitor.TryEnter(_lockObject, TimeSpan.FromSeconds(1)))
            UnityEngine.Debug.LogException(new Exception("Deadlock"));

        try
        {
            if (_objects.Count == 0)
                return null;

            //UnityEngine.Debug.Log("UnityEngineThread: Dequeue - " + (_objects.Count - 1));
            return _objects.Dequeue();
        }
        finally
        {
            Monitor.Exit(_lockObject);
        }
	}

	private Queue<Action> _objects = new Queue<Action>();
	private readonly object _lockObject = new object();
	private static WeakReference<UnityEngineThread> _self;
}
