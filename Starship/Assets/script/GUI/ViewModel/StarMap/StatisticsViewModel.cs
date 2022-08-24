using Session;
using UnityEngine;
using Zenject;

namespace ViewModel
{
	public class StatisticsViewModel : MonoBehaviour
	{
	    [Inject] private readonly ISessionData _session;

		public TextFieldViewModel VisitedStars;

		private void OnEnable()
		{
			VisitedStars.Value.text = _session.StarMap.VisitedStarsCount.ToString();
		}
	}
}
