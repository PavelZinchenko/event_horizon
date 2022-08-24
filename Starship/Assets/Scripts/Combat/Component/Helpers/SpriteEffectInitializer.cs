using GameDatabase.DataModel;
using Services.Reources;
using UnityEngine;

namespace Combat.Component.Helpers
{
    public class SpriteEffectInitializer : MonoBehaviour, IEffectPrefabInitializer
    {
        [SerializeField] private SpriteRenderer[] _renderers;

        public void Initialize(VisualEffectElement data, IResourceLocator resourceLocator)
        {
            if (_renderers == null || _renderers.Length == 0) return;

            var sprite = resourceLocator.GetSprite(data.Image);
            foreach (var item in _renderers)
                item.sprite = sprite;
        }
    }
}
