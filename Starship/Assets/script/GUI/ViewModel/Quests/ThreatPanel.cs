using UnityEngine;
using UnityEngine.UI;
using GameServices.Player;
using Services.Localization;
using Zenject;

namespace ViewModel
{
	namespace Quests
	{
		public class ThreatPanel : MonoBehaviour
		{
		    [Inject] private readonly PlayerFleet _playerFleet;
		    [Inject] private readonly ILocalization _localization;

			public Text ThreatText;
			public Graphic ThreatImage;
			public Color[] ThreatColors;

			public void Initialize(float threat)
			{
				if (threat > 0)
				{
					gameObject.SetActive(true);
					UpdateThreatInfo(threat);
				}
				else
				{
					gameObject.SetActive(false);
				}
			}		
			
			private void UpdateThreatInfo(float enemyPower)
			{
				var level = (int)Maths.Threat.GetLevel(_playerFleet.Power, enemyPower);

				if (ThreatText != null) ThreatText.text = _localization.GetString("$ThreatText", _localization.GetString("$ThreatLevel" + (level+1)));
				if (ThreatImage != null)
					ThreatImage.color = ThreatColors[level];
			}
		}
	}
}
