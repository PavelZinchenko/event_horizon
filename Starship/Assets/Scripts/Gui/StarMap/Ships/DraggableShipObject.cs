using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.StarMap
{
    public class DraggableShipObject : MonoBehaviour
    {
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private Image _icon;

        public void Initialize(ShipListItem item)
        {
            _icon.sprite = _resourceLocator.GetSprite(item.Ship.Model.ModelImage);
            _icon.color = item.Ship.ColorScheme.HsvColor;
        }
    }
}
