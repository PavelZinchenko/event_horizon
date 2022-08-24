using GameDatabase.DataModel;
using Services.Reources;

namespace Combat.Component.Helpers
{
    public interface IEffectPrefabInitializer
    {
        void Initialize(VisualEffectElement data, IResourceLocator resourceLocator);
    }
}
