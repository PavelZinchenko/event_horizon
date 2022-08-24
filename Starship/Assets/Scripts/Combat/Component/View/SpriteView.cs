using GameDatabase.Enums;
using GameDatabase.Extensions;
using UnityEngine;

namespace Combat.Component.View
{
    public class SpriteView : BaseView
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _alphaScale = 1.0f;
        [SerializeField] private bool _lifeAffectsOpacity = false;
        [SerializeField] private bool _lifeAffectsSize = false;
        [SerializeField] private Color _baseColor = Color.white;
        [SerializeField] private ColorMode _colorMode = ColorMode.TakeFromOwner;

        public void Initialize(Sprite sprite, Color color, ColorMode colorMode)
        {
            if (sprite) _spriteRenderer.sprite = sprite;
            _baseColor = color;
            _colorMode = colorMode;
        }

        public override void Dispose()
        {
            Opacity = 1.0f;
            Scale = 1.0f;
        }

        protected override void UpdateLife(float life)
        {
            if (_lifeAffectsOpacity)
                Opacity = 1f - (1f - life) * (1f - life);

            if (_lifeAffectsSize)
                Scale = 1f - (1f - life)*(1f - life);
        }

        protected override void UpdatePosition(Vector2 position)
        {
            _spriteRenderer.transform.localPosition = position;
        }

        protected override void UpdateRotation(float rotation)
        {
            _spriteRenderer.transform.localEulerAngles = new Vector3(0,0,rotation);
        }

        protected override void UpdateSize(float size)
        {
            _spriteRenderer.transform.localScale = _initialSize*size*Vector3.one;
        }

        protected override void UpdateColor(Color color)
        {
            color = _colorMode.Apply(_baseColor, color);
            color.a *= _alphaScale;
            _spriteRenderer.material.color = color;
        }

        protected override void OnGameObjectCreated()
        {
            _initialSize = _spriteRenderer.transform.localScale.z;
        }

        protected override void OnGameObjectDestroyed()
        {
            foreach (var item in _spriteRenderer.materials)
                GameObject.DestroyImmediate(item);
        }

        protected SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }

        private float _initialSize;
    }
}
