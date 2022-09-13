using UnityEngine;
using UnityEngine.UI;

namespace ViewModel
{
    public class StatsBlockViewModel : MonoBehaviour
    {
        [SerializeField] private LayoutElement _separator;

        public float SeparatorHeight
        {
            get => _separator.preferredHeight;
            set => _separator.preferredHeight = value;
        }
    }
}
