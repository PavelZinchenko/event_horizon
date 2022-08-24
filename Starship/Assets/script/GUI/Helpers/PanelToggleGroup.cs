using UnityEngine;

namespace Gui
{
    public class PanelToggleGroup : MonoBehaviour
    {
        public GameObject[] Panels;

        public void Show(GameObject panel)
        {
            foreach (var item in Panels)
                item.SetActive(item == panel);
        }
    }
}
