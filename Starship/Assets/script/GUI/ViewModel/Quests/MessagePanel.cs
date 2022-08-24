using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameModel.Quests;
using Zenject;
using Services.Localization;

namespace ViewModel
{
	namespace Quests
	{
		public class MessagePanel : MonoBehaviour
		{
			[Inject] private readonly ILocalization _localization;

			//public GameObject IconPanel;
			//public Image Icon;
			public Text MessageText;
			
			public void Initialize(string text)
			{
				if (string.IsNullOrEmpty(text))
				{
					gameObject.SetActive(false);
					return;
				}

				gameObject.SetActive(true);
				//if (IconPanel == null)
				//{
				//	MessageText.alignment = TextAnchor.MiddleCenter;
				//}
				//else if (character != null && character.Icon != null)
				//{
				//	IconPanel.SetActive(true);
				//	Icon.sprite = character.Icon;
				//	Icon.color = character.IconColor;
				//	MessageText.alignment = TextAnchor.UpperLeft;
				//}
				//else
				//{
				//	IconPanel.SetActive(false);
				//	MessageText.alignment = TextAnchor.MiddleCenter;
				//}

				MessageText.text = _localization.GetString(text);
			}
		}
	}
}
