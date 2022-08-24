using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Gui
{
	[RequireComponent (typeof(Toggle))]
	public class ToggleSelectedEvent : MonoBehaviour
	{
		public UnityEvent onButtonSelected = new UnityEvent();
		public UnityEvent onButtonDeselected = new UnityEvent();

		private void Awake()
		{
			GetComponent<Toggle>().onValueChanged.AddListener(valueChanged);
		}
		
		private void valueChanged(bool value)
		{
			if (value)
				onButtonSelected.Invoke();
			else
				onButtonDeselected.Invoke();
		}
	}
}