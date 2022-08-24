using GameDatabase.DataModel;
using Services.Reources;

namespace Combat.Component.Helpers
{
    public interface IBulletPrefabInitializer
    {
        void Initialize(BulletPrefab data, IResourceLocator resourceLocator);
    }
}
