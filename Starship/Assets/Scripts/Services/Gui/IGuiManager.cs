using System;

namespace Services.Gui
{
    public interface IGuiManager
    {
        void OpenWindow(string id, Action<WindowExitCode> onCloseAction = null);
		void OpenWindow(string id, WindowArgs args, Action<WindowExitCode> onCloseAction = null);
        void CloseAllWindows();
        bool AutoWindowsAllowed { get; set; }
    }
}
