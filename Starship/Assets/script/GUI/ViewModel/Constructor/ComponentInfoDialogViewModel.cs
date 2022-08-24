using UnityEngine;

namespace ViewModel
{
	public class ComponentInfoDialogViewModel : MonoBehaviour
	{
		public CanvasGroup UiCanvasGroup;
		public ComponentViewModel Component;

		public void Open(Constructor.ComponentInfo component)
		{
			UiCanvasGroup.blocksRaycasts = false;
			UiCanvasGroup.interactable = false;
			GetComponent<PanelController>().Open();
			Component.Initialize(component, 0);
		}
		
		public void OnClosing()
		{
			UiCanvasGroup.blocksRaycasts = true;
			UiCanvasGroup.interactable = true;
		}
	}
}
