using UnityEngine;

namespace Combat.Component.View
{
    public class EmptyView : IView
    {
        public void ApplyHsv(float hue, float saturation) { throw new System.NotImplementedException(); }
        public void Dispose() {}

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }
        public float Life { get; set; }
        public void UpdateView(float elapsedTime) {}
    }
}
