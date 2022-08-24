using Constructor;
using UnityEngine;

namespace Gui.ComponentList
{
    public abstract class ComponentListItemBase : MonoBehaviour
    {
        public abstract void Initialize(ComponentInfo item, int quantity);
        public abstract ComponentInfo Component { get; }
        public abstract bool Selected { get; set; }
    }
}
