using Combat.Component.Mods;
using UnityEngine;

namespace Combat.Component.Features
{
    public struct FeaturesData
    {
        public TargetPriority TargetPriority;
        public Color Color;
        public float Opacity;
        public bool Invulnerable;
    }

    public interface IFeaturesModification : IModification<FeaturesData> {}
}
