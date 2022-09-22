using System;
using Utils;

namespace GameDatabase.Model
{
    public struct ColorData
    {
        public ColorData(string color)
        {
            _color = FromString(color);
        }

        public ColorData(UnityEngine.Color color)
        {
            _color = color;
        }

        public static implicit operator ColorData(UnityEngine.Color color)
        {
            return new ColorData(color);
        }

        public static implicit operator UnityEngine.Color(ColorData data)
        {
            return data._color;
        }

        public float R => _color.r;
        public float G => _color.g;
        public float B => _color.b;
        public float A => _color.a;

        private static UnityEngine.Color FromString(string color)
        {
            if (string.IsNullOrEmpty(color) || color[0] != '#')
                return UnityEngine.Color.white;

            uint value;

            try
            {
                value = Convert.ToUInt32(color.Substring(1), 16);
            }
            catch (Exception e)
            {
                OptimizedDebug.LogException(e);
                return UnityEngine.Color.white;
            }

            if (value <= 0xffffff)
                value |= 0xff000000;

            var b = (byte)(value & 0xff); value >>= 8;
            var g = (byte)(value & 0xff); value >>= 8;
            var r = (byte)(value & 0xff); value >>= 8;
            var a = (byte)(value & 0xff);

            return new UnityEngine.Color32(r, g, b, a);
        }

        public override string ToString()
        {
            var c = (UnityEngine.Color32)_color;
            return "#" + (c.a < 0xff ? c.a.ToString("X2") : string.Empty) + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
        }

        private readonly UnityEngine.Color _color;
    }
}
