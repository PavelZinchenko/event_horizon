using Combat.Effects;
using GameDatabase.DataModel;
using Services.Reources;
using UnityEngine;

namespace Combat.Component.Helpers
{
    public class WaveEffectInitializer : MonoBehaviour, IEffectPrefabInitializer
    {
        [SerializeField] private WaveEffect _waveEffect;

        public void Initialize(VisualEffectElement data, IResourceLocator resourceLocator)
        {
            var sprite = resourceLocator.GetSprite(data.Image);
            _waveEffect.Texture = sprite != null ? sprite.texture : null;
            _waveEffect.SetSize(0.5f);
        }
    }
}
