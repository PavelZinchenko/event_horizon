using Combat.Helpers;

namespace Combat.Effects
{
    public interface IEffectComponent : IEffect
    {
        void Initialize(GameObjectHolder objectHolder);
    }
}
