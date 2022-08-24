using System.Collections.Generic;
using Constructor.Ships.Modification;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.StarMap
{
    public class ModsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject[] _panels;
        [SerializeField] private Image[] _icons;

        public void Initialize(IEnumerable<IShipModification> modifications, IResourceLocator resourceLocator)
        {
            var enumerator = modifications.GetEnumerator();

            for (var i = 0; i < _panels.Length; ++i)
            {
                if (!enumerator.MoveNext() || enumerator.Current == null)
                {
                    _panels[i].gameObject.SetActive(false);
                    continue;
                }

                _panels[i].gameObject.SetActive(true);
                var mod = enumerator.Current;

                var sprite = resourceLocator.GetSprite(mod.Type.GetIconId());
                _icons[i].gameObject.SetActive(sprite != null);
                _icons[i].sprite = sprite;
                _icons[i].color = mod.Type.GetColor();
            }
        }
    }
}
