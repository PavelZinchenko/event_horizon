using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(TextMesh))]
    public class TextEffect : EffectBase
    {
        protected override void OnInitialize()
        {
            _textMesh = GetComponent<TextMesh>();
            _textMesh.text = gameObject.name;
        }

        protected override void OnBeforeUpdate()
        {
            Scale = UnityEngine.Camera.main.orthographicSize;
        }

        protected override void SetColor(Color color)
        {
            _textMesh.color = color;
        }

        protected override void OnDispose() {}
        protected override void OnGameObjectDestroyed() {}
        protected override void UpdateLife() {}

        private TextMesh _textMesh;
    }
}
