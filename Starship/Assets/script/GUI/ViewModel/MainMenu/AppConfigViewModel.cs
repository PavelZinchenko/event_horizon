using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
	public class AppConfigViewModel : MonoBehaviour
	{
		[SerializeField] private Text _versionText;

		private void Start()
		{
    		_versionText.text = AppConfig.version + " (build " + AppConfig.buildNumber + ")";
		}
	}
}
