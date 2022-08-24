using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel
{
	public class MessagePanelViewModel : MonoBehaviour
	{
		public Text TextArea;

		public void Open(string text)
		{
			//Combat.Manager.Instance.Paused = true;
			//GetComponent<PanelController>().Open();
			//TextArea.text = text;
		}

		public void Close()
		{
			//Combat.Manager.Instance.Paused = false;
			//GetComponent<PanelController>().Close();
		}
	}
}
