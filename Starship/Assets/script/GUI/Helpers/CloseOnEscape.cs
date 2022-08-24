using Services.Gui;
using UnityEngine;

namespace Gui
{
    [RequireComponent(typeof(IWindow))]
    public class CloseOnEscape : MonoBehaviour
    {
        private void Update()
        {
            if (Window.Enabled && Input.GetKeyDown(KeyCode.Escape))
                Window.Close();
        }

        private IWindow Window { get { return _window ?? (_window = GetComponent<IWindow>()); } }
        private IWindow _window;
    }
}
