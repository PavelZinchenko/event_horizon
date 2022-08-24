using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class TimerViewModel : MonoBehaviour
	{
		public Text Label;

		public void SetTime(long time)
		{
			if (time <= 0)
			{
				gameObject.SetActive(false);
				return;
			}				

			gameObject.SetActive(true);
			Label.gameObject.SetActive(true);
			var seconds = (time/System.TimeSpan.TicksPerSecond) % 60;
			var minutes = (time/System.TimeSpan.TicksPerMinute) % 60;
			var hours = (time/System.TimeSpan.TicksPerHour) % 24;
			var days = time/System.TimeSpan.TicksPerDay;

			if (days > 0)
				Label.text = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", days, hours, minutes, seconds);
			else if (hours > 0)
				Label.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
			else
				Label.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}

		private const long ticksPerSecond = System.TimeSpan.TicksPerSecond;
		private const long ticksPerMinute = System.TimeSpan.TicksPerMinute;
	}
}
