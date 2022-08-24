using GameDatabase.DataModel;
using Services.Reources;
using UnityEngine;

namespace Combat.Component.Helpers
{
    public class EmptyInitializer : MonoBehaviour, IBulletPrefabInitializer
    {
        public void Initialize(BulletPrefab data, IResourceLocator resourceLocator) {}
    }
}
