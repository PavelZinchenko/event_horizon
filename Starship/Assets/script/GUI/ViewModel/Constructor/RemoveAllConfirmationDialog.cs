using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ViewModel
{
	public class RemoveAllConfirmationDialog : MonoBehaviour
	{
		public CanvasGroup UiCanvasGroup;

		public RemoveAllConfirmationDialog()
		{
			_instance = new WeakReference<RemoveAllConfirmationDialog>(this);
		}
		
		public static void Open(System.Action action)
		{
			_instance.Target.Initialize(action);
		}
		
		public void ConfirmButtonClicked()
		{
			GetComponent<PanelController>().Close();
			_action.Invoke();
		}
		
		public void OnClosing()
		{
			UiCanvasGroup.blocksRaycasts = true;
			UiCanvasGroup.interactable = true;
		}
		
		private void Initialize(System.Action action)
		{
			_action = action;
			GetComponent<PanelController>().Open();
			UiCanvasGroup.blocksRaycasts = false;
			UiCanvasGroup.interactable = false;
		}
		
		private System.Action _action;
		private static WeakReference<RemoveAllConfirmationDialog> _instance;
	}
}
