using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Model;

namespace ViewModel
{
	public class ButtonCooldown : MonoBehaviour
	{
		public Image HardCooldownImage;
		public Image SoftCooldownImage;

		public void SetCooldown(float min, float max)
		{
			if (min > 0f)
			{
				HardCooldownImage.enabled = true;
				HardCooldownImage.fillAmount = min;
			}
			else
			{
				HardCooldownImage.enabled = false;
			}

			/*if (max > 0f && max != min)
			{
				SoftCooldownImage.enabled = true;
				SoftCooldownImage.fillAmount = max;
			}
			else
			{
				SoftCooldownImage.enabled = false;
			}*/
		}
	}
}
