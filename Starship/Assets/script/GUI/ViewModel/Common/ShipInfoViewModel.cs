using GameDatabase.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class ShipInfoViewModel : MonoBehaviour
	{
		public Toggle Toggle;
		public Button Button;
		public Image Icon;
		public Text LevelText;
		public Text NameText;
	    public Text ClassText;
		public RectTransform LevelPanel;
		public LayoutGroup ClassPanel;
		public Slider ConditionSlider;

		public void SetLevel(int level)
		{
			LevelText.text = level > 0 ? level.ToString() : "0";
			if (LevelPanel != null)
				LevelPanel.gameObject.SetActive(level > 0);
		}

		public void SetClass(DifficultyClass shipClass)
		{
			if (shipClass <= DifficultyClass.Default)
			{
				ClassPanel.gameObject.SetActive(false);
				return;
			}

			ClassPanel.gameObject.SetActive(true);
			int index = 0;
			foreach (Transform child in ClassPanel.transform)
			{
				var image = child.GetComponent<Image>();
				if (image == null) 
					continue;
				
				image.gameObject.SetActive(index++ < (int)shipClass);
			}
		}
	}
}
