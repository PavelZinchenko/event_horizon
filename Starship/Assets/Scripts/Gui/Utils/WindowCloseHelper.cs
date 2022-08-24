using Gui.Windows;
using Services.Gui;
using UnityEngine;

namespace Gui.Utils
{
    public class WindowCloseHelper : MonoBehaviour
    {
        public void CloseWithResultOk()
        {
            Window.Close(WindowExitCode.Ok);
        }

        public void CloseWithResultOption1()
        {
            Window.Close(WindowExitCode.Option1);
        }

        public void CloseWithResultOption2()
        {
            Window.Close(WindowExitCode.Option2);
        }

        private AnimatedWindow Window { get { return GetComponent<AnimatedWindow>(); } }
    }
}
