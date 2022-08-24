using UnityEngine;

namespace Constructor.Model
{
    public struct ColorScheme
    {
        public ColorScheme(Color color, float hue = 0, float saturation = 0)
        {
            _initialized = true;
            _color = color;
            Hue = hue;
            Saturation = saturation;
        }

        public Color Color => _initialized ? _color : Color.white;
        public readonly float Hue;
        public readonly float Saturation;

        public bool IsHsv => _initialized && Hue >= 0.01f && Hue <= 0.99f || Saturation > 0.01f;

        private readonly bool _initialized;
        private readonly Color _color;
    }

}
