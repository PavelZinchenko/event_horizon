using System.Collections;
using UnityEngine;

namespace ViewModel
{
	[RequireComponent(typeof(PanelController))]
	public class PanelControllerAutoClose : MonoBehaviour
	{
		[SerializeField] float Timeout = 5f;

		private void OnEnable()
		{
			_elapsedTime = 0;
		}

		private void Update()
		{
			if (_elapsedTime > Timeout)
				return;

			_elapsedTime += Time.unscaledDeltaTime;
			if (_elapsedTime > Timeout)
			{
				GetComponent<PanelController>().Close();
			}
		}

		private float _elapsedTime;
	}
}
