using Combat.Component.View;
using GameDatabase.DataModel;
using Services.Reources;
using UnityEngine;

namespace Combat.Component.Helpers
{
    public class ProjectileInitializer : MonoBehaviour, IBulletPrefabInitializer
    {
        [SerializeField] private SpriteView BulletView;
        [SerializeField] private SpriteView EngineView;
        [SerializeField] private TrailView TrailView;
        [SerializeField] private Transform BulletObject;
        [SerializeField] private Transform EngineObject;
        [SerializeField] private Transform TrailObject;

        public void Initialize(BulletPrefab data, IResourceLocator resourceLocator)
        {
            if (BulletView) BulletView.Initialize(resourceLocator.GetSprite(data.Image), data.MainColor, data.MainColorMode);
            if (BulletObject) BulletObject.localScale = Vector3.one * data.Size;

            if (EngineView) EngineView.Initialize(null, data.SecondColor, data.SecondColorMode);
            if (EngineObject) EngineObject.localPosition = new Vector3(-data.Margins, 0, 0);

            if (TrailView) TrailView.Initialize(data.SecondColor, data.SecondColorMode);
            if (TrailObject) TrailObject.localPosition = new Vector3(-data.Margins - 0.1f, 0, 0);
        }
    }
}
