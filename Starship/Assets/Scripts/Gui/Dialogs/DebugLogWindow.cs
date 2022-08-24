using System.Collections.Generic;
using Services.Gui;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Dialogs
{
    public class DebugLogWindow : MonoBehaviour
    {
        [SerializeField] private LayoutGroup _content;

        public void InitializeWindow(WindowArgs args)
        {
            for (var i = 0; i < args.Count; ++i)
                _messages.Add(args.Get<object>(i).ToString());

            _content.InitializeElements<Text, string>(_messages, UpdateLine);
        }

        public void OnWindowClosed()
        {
            _messages.Clear();
        }

        private void UpdateLine(Text text, string data)
        {
            text.text = data;
        }

        private readonly List<string> _messages = new List<string>();
    }
}
