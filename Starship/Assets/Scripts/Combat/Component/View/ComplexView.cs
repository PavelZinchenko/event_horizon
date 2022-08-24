using UnityEngine;

namespace Combat.Component.View
{
    public class ComplexView : BaseView
    {
        [SerializeField] private BaseView[] _views;

        public override void UpdateView(float elapsedTime)
        {
            base.UpdateView(elapsedTime);
            foreach (var view in _views)
                view.UpdateView(elapsedTime);
        }

        public override void Dispose()
        {
            foreach (var view in _views)
                view.Dispose();
        }

        protected override void OnGameObjectCreated() {}
        protected override void OnGameObjectDestroyed() {}

        protected override void UpdateLife(float life)
        {
            foreach (var view in _views)
                view.Life = life;
        }

        protected override void UpdatePosition(Vector2 position)
        {
            //foreach (var view in _views)
            //    view.Position = position;
        }

        protected override void UpdateRotation(float rotation)
        {
            //foreach (var view in _views)
            //    view.Rotation = rotation;
        }

        protected override void UpdateSize(float size)
        {
            //foreach (var view in _views)
            //    view.Size = size;
        }

        protected override void UpdateColor(Color color)
        {
            foreach (var view in _views)
                view.Color = color;
        }
    }
}
