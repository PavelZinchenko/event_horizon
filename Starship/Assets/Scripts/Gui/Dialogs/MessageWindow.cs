using Services.Gui;
using Services.Storage;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Gui.Common
{
    public class MessageWindow : MonoBehaviour
    {
        [SerializeField] private Text _text;

        public void InitializeWindow(WindowArgs args)
        {
            _text.text = args.Get<string>();
        }
    }
}
