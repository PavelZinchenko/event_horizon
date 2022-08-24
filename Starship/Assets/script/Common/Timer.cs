using System;

public class Timer 
{
	public Timer(TimeSpan interval)
	{
		_interval = interval.Ticks;
		_lastUpdateTime = DateTime.Now.Ticks;
	}

	public static bool Paused;

	public bool Triggered
	{
		get
		{
			if (Paused)
			{
				_lastUpdateTime = DateTime.Now.Ticks;
				return false;
			}

			return DateTime.Now.Ticks - _lastUpdateTime >= _interval; 
		}
	}

	public TimeSpan GetElapsedTimeAndUpdate()
	{
		var time = DateTime.Now.Ticks;
		var elapsed = TimeSpan.FromTicks(time - _lastUpdateTime);
		_lastUpdateTime = time;
		return Paused ? TimeSpan.Zero : elapsed;
	}

	public TimeSpan TimeLeft
	{
		get
		{
			if (Paused)
			{
				_lastUpdateTime = DateTime.Now.Ticks;
				return TimeSpan.FromTicks(_interval);
			}

			var elapsed = DateTime.Now.Ticks - _lastUpdateTime;
			return _interval > elapsed ? TimeSpan.FromTicks(_interval-elapsed) : TimeSpan.Zero;
		}
	}

	private long _interval;
	private long _lastUpdateTime;
}
