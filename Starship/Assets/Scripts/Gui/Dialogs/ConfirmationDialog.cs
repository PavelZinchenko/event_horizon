using Gui.Windows;
using Services.Gui;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Dialogs
{
    public class ConfirmationDialog : MonoBehaviour
    {
        [SerializeField] private Text _text;

        public void InitializeWindow(WindowArgs args)
        {
            _text.text = args.Get<string>();
        }

        public void ConfirmButtonClicked()
        {
            GetComponent<AnimatedWindow>().Close(WindowExitCode.Ok);
        }
    }
}
