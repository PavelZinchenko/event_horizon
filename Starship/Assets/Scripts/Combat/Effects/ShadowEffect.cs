using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShadowEffect : EffectBase
    {
        [SerializeField] private float _effectSize;
        [SerializeField] private Vector2 _offset;
        private static readonly int Size1 = Shader.PropertyToID("_Size");

        protected override void OnInitialize()
        {
            Scale = _effectSize;
            Renderer.sprite = transform.parent.GetComponent<SpriteRenderer>().sprite; // TODO: temporary fix
        }

        protected override void OnDispose()
        {
            _renderer = null;
        }
        protected override void OnGameObjectDestroyed() { }

        protected override void OnAfterUpdate()
        {
            var parent = Transform.parent;
            Transform.localPosition = Position + RotationHelpers.Transform(_offset, -parent.localEulerAngles.z) / parent.localScale.z;
        }

        protected override void UpdateLife()
        {
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();

            var size = transform.lossyScale.z;
            Renderer.material.SetFloat(Size1, 0.1f / size);
        }
        
        private SpriteRenderer _renderer;
        // ReSharper disable once Unity.NoNullCoalescing
        private SpriteRenderer Renderer => _renderer ?? (_renderer = GetComponent<SpriteRenderer>());
        
        private Transform _transform;
        // ReSharper disable once Unity.NoNullCoalescing
        private Transform Transform => _transform ?? (_transform = GetComponent<Transform>());
    }
}
