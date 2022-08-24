using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel
{
	public class ControlPanelViewModel : MonoBehaviour
	{
		public PanelController CargoHoldPanel;
		public PanelController UpgradePanel;
		public PanelController JournalPanel;

		public void ScrapButtonClicked()
		{
			if (CargoHoldPanel.IsVisible)
			{
				CargoHoldPanel.Close();
			}
			else
			{
				OpenPanel(CargoHoldPanel);
			}
		}
		
		public void UpgradeButtonClicked()
		{
			if (UpgradePanel.IsVisible)
			{
				UpgradePanel.Close();
			}
			else
			{
				OpenPanel(UpgradePanel);
			}
		}

		public void JournalButtonClicked()
		{
			if (JournalPanel.IsVisible)
			{
				JournalPanel.Close();
			}
			else
			{
				OpenPanel(JournalPanel);
			}
		}

		private void OpenPanel(PanelController panel)
		{
			if (panel == UpgradePanel)
				UpgradePanel.Open();
			else
				UpgradePanel.Close();

			if (panel == CargoHoldPanel)
				CargoHoldPanel.Open();
			else
				CargoHoldPanel.Close();

			if (panel == JournalPanel)
				JournalPanel.Open();
			else
				JournalPanel.Close();
		}
	}
}
