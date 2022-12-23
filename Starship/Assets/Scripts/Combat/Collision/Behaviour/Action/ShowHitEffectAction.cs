using Combat.Collision.Manager;
using Combat.Component.Unit;
using Combat.Effects;
using GameDatabase.DataModel;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Collision.Behaviour.Action
{
    public class ShowHitEffectAction : ICollisionAction
    {
        public ShowHitEffectAction(Factory.EffectFactory effectFactory, PrefabId prefabId, Color color, float size, float lifetime = 0.2f)
        {
            _effectFactory = effectFactory;
            _prefabId = prefabId;
            _color = color;
            _size = size;
            _lifetime = lifetime;
        }

        public ShowHitEffectAction(Factory.EffectFactory effectFactory, VisualEffect effectData, Color color, float size, float lifetime = 0.2f)
        {
            _effectFactory = effectFactory;
            _visualEffect = effectData;
            _color = color;
            _size = size;
            _lifetime = lifetime;
        }

        public void Invoke(IUnit self, IUnit target, CollisionData collisionData, ref Impact selfImpact, ref Impact targetImpact)
        {
            if (_effect == null || !_effect.IsAlive)
            {
                _effect = _visualEffect != null ? _effectFactory.CreateCompositeEffect(_visualEffect, null) : _effectFactory.CreateEffect(_prefabId);
                _effect.Color = _color;
                _effect.Size = _size;
            }

            _effect.Position = collisionData.Position;
            _effect.Run(_lifetime, target.Body.Velocity, 0);
        }

        public void Dispose() {}

        private IEffect _effect;
        private readonly float _lifetime;
        private readonly Color _color;
        private readonly float _size;
        private readonly Factory.EffectFactory _effectFactory;
        private readonly PrefabId _prefabId;
        private readonly VisualEffect _visualEffect;
    }
}
