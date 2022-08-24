using UnityEngine;
using UnityEngine.UI;

namespace Gui.MainMenu
{
    public class NewSavedGameItem : MonoBehaviour
    {
        [SerializeField] private InputField _name;
        [SerializeField] private Toggle _toggle;

        public bool IsSelected { get { return _toggle.isOn; } }
        public string Name { get { return _name.text; } }

        public void OnValueChanged(string value)
        {
            if (!_toggle.isOn)
                _toggle.isOn = true;
        }

        public void OnEndEdit()
        {
            if (string.IsNullOrEmpty(_name.text))
                CreateDefaultName();
        }

        private void OnEnable()
        {
            CreateDefaultName();
        }

        private void CreateDefaultName()
        {
            _name.text = "Savegame " + System.DateTime.Now;
        }
    }
}
