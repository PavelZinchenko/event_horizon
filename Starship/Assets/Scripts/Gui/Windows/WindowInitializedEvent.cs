using Services.Gui;
using UnityEngine.Events;

namespace Gui.Windows
{
    [System.Serializable]
    public class WindowInitializedEvent : UnityEvent<WindowArgs> { }

    [System.Serializable]
    public class UnityEventBool : UnityEvent<bool> { }
}
