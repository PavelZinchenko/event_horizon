using System.Collections;
using UnityEngine;

namespace Gui.Windows
{
    public abstract class AnimatedWindowBase : MonoBehaviour
    {
        protected void OpenWindow()
        {
            if (_isOpen) return;
            _isOpen = true;

            gameObject.SetActive(true);

            OnWindowOpening();

            StartAnimation();
        }

        protected void CloseWindow()
        {
            if (!_isOpen) return;
            _isOpen = false;

            OnWindowClosing();

            StartAnimation();
        }

        protected bool IsWindowVisible { get { return _isOpen; } }

        protected virtual void Start()
        {
            if (_initiallyOpen)
            {
                OnWindowOpening();
                OnWindowOpened();
            }
        }

        protected virtual void OnEnable()
        {
            if (!_isOpen)
                _initiallyOpen = _isOpen = true;

            if (gameObject.activeSelf)
                StartAnimation();
        }

        private void StartAnimation()
        {
            Animator.SetBool(IsVisibleKey, _isOpen);

            if (!_isRunning && gameObject.activeSelf)
                StartCoroutine(WaitAnimationDone());
            else if (_isOpen)
                OnWindowOpened();
            else
                OnWindowClosed();
        }

        private IEnumerator WaitAnimationDone()
        {
            _isRunning = true;
            System.Action action;

            while (true)
            {
                if (!Animator.IsInTransition(0))
                {
                    var isVisible = Animator.GetBool(IsVisibleKey);
                    if (isVisible && Animator.GetCurrentAnimatorStateInfo(0).IsName(OpenedStateName))
                    {
                        action = OnWindowOpened;
                        break;
                    }
                    else if (!isVisible && Animator.GetCurrentAnimatorStateInfo(0).IsName(ClosedStateName))
                    {
                        action = OnWindowClosed;
                        gameObject.SetActive(false);
                        break;
                    }
                }

                yield return null;
            }

            _isRunning = false;
            action.Invoke();
        }

        protected abstract void OnWindowOpened();
        protected abstract void OnWindowClosed();
        protected abstract void OnWindowOpening();
        protected abstract void OnWindowClosing();

        private Animator Animator { get { return _animator ?? (_animator = GetComponent<Animator>()); } }
        private Animator _animator;
        private bool _isRunning;
        private bool _isOpen;
        private bool _initiallyOpen;

        private const string IsVisibleKey = "IsVisible";
        private const string OpenedStateName = "Open";
        private const string ClosedStateName = "Closed";
    }
}
