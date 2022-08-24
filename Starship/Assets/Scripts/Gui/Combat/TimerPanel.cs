using Gui.Windows;
using Services.Gui;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Combat
{
    class TimerPanel : MonoBehaviour
    {
        [SerializeField] private Text _textArea;
        [SerializeField] private AnimatedWindow _combatMenu;

        public bool Enabled
        {
            get { return GetComponent<IWindow>().IsVisible; }
            set
            {
                if (value)
                    Window.Open();
                else
                    Window.Close(WindowExitCode.Ok);
            }
        }

        public int Time
        {
            get { return _time; }
            set
            {
                if (_time == value)
                    return;

                _time = value;
                _textArea.text = _time.ToString("D2");
                _textArea.gameObject.SetActive(_time > 0);
            }
        }

        public void PauseButtonClicked()
        {
            _combatMenu.Open();
        }

        private AnimatedWindow Window { get { return _window ?? (_window = GetComponent<AnimatedWindow>()); } }

        private int _time = -1;
        private AnimatedWindow _window;
    }
}
