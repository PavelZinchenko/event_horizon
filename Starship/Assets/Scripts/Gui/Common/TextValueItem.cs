using UnityEngine;
using UnityEngine.UI;

namespace Gui.Common
{
    public class TextValueItem : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private Text _value;

        public string Value { get { return _value.text; } set { _value.text = value; } }
    }
}
