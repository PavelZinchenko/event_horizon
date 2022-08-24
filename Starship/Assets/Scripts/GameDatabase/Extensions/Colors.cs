using System;
using GameDatabase.Enums;
using UnityEngine;

namespace GameDatabase.Extensions
{
    public static class ColorExtensions
    {
        public static Color Apply(this ColorMode mode, Color newColor, Color parentColor)
        {
            switch (mode)
            {
                case ColorMode.UseMyOwn: return newColor;
                case ColorMode.Blend: return Color.Lerp(parentColor, newColor, 0.5f);
                case ColorMode.Multiply: return parentColor*newColor;
                case ColorMode.TakeFromOwner: return parentColor;
                default: throw new ArgumentException();
            }
        }

    }
}
