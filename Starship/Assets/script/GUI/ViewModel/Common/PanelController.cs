using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ViewModel
{
	public class PanelController : MonoBehaviour
	{
		public UnityEvent OnWindowClosedEvent = new UnityEvent();
		public UnityEvent OnWindowClosingEvent = new UnityEvent();
        public UnityEvent OnWindowOpenedEvent = new UnityEvent();

        public void Open()
		{
			if (_isOpen) return;
            _isOpen = true;

            if (gameObject.activeSelf)
				gameObject.SendMessage("OnEnable", SendMessageOptions.DontRequireReceiver);
			else
				gameObject.SetActive(true);

            StartAnimation();
		}

		public void Close()
		{
			if (!_isOpen) return;
            _isOpen = false;

			OnWindowClosingEvent.Invoke();

            StartAnimation();
		}

		public void Toggle()
		{
            if (_isOpen)
				Close();
			else
				Open();
		}

		public bool IsVisible { get { return _isOpen; } }
		public bool IsRunning { get { return _isRunning; } }

		private void OnEnable()
		{
			if (gameObject.activeSelf)
				StartAnimation();
		}

        private void StartAnimation()
	    {
            Animator.SetBool(IsVisibleKey, _isOpen);

            if (!_isRunning && gameObject.activeSelf)
                StartCoroutine(WaitAnimationDone());
        }

        private IEnumerator WaitAnimationDone()
		{
			_isRunning = true;
			UnityEvent windowEvent;

			while (true)
			{
				if (!Animator.IsInTransition(0))
				{
					var isVisible = Animator.GetBool(IsVisibleKey);
					if (isVisible && Animator.GetCurrentAnimatorStateInfo(0).IsName(OpenedStateName))
					{
						windowEvent = OnWindowOpenedEvent;
						break;
					}
					else if (!isVisible && Animator.GetCurrentAnimatorStateInfo(0).IsName(ClosedStateName))
					{
						windowEvent = OnWindowClosedEvent;
						gameObject.SetActive(false);
						break;
					}
				}

				yield return null;
			}

			_isRunning = false;
			windowEvent.Invoke();
		}

		private Animator Animator {	get { return _animator ?? (_animator = GetComponent<Animator>()); }	}
		private Animator _animator;
		private bool _isRunning;
	    private bool _isOpen;

		private const string IsVisibleKey = "IsVisible";
		private const string OpenedStateName = "Open";
		private const string ClosedStateName = "Closed";
	}
}
