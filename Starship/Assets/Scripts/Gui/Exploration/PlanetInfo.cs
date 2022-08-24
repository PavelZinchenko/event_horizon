using Game.Exploration;
using UnityEngine;
using UnityEngine.UI;
using GameServices.Random;
using Services.Localization;
using Zenject;

namespace Gui.Exploration
{
	public class PlanetInfo : MonoBehaviour
	{
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IRandom _random;

		public Image Icon;
	    public Image Mask;
		public Text NameText;
		public Text TypeText;
	    public GameObject ExploredMark;

        public void UpdatePlanet(Planet planet)
		{
		    if (Mask != null)
		    {
                if (planet.Type == PlanetType.Infected)
                {
                    Mask.gameObject.SetActive(false);
                }
                else
                {
                    Mask.gameObject.SetActive(true);
                    Mask.transform.localEulerAngles = new Vector3(0, 0, _random.RandomInt(planet.StarId + planet.Index, 360));
                    Mask.color = Color.Lerp(Star.GetStarColor(planet.StarId), new Color(1f, 1f, 1f, 0f), 0.6f);

			    }
            }

			if (ExploredMark)
		        ExploredMark.SetActive(planet.ObjectivesExplored == planet.TotalObjectives);

            NameText.text = planet.Name;
			TypeText.text = planet.Type.Name(_localization);
			Icon.sprite = planet.Icon;
			Icon.rectTransform.localScale = Vector3.one*(planet.Size + 1f)*0.5f;
		}
	}
}
