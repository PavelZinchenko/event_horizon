using System;
using UnityEngine;

namespace Combat.Component.View
{
    public interface IView : IDisposable
    {
        void ApplyHsv(float hue, float saturation);

        Vector2 Position { get; set; }
        float Rotation { get; set; }
        float Size { get; set; }
        Color Color { get; set; }
        float Life { get; set; }

        void UpdateView(float elapsedTime);
    }
}
