using System;
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

        private void Start()
        {
            _mainCamera = UnityEngine.Camera.main;
        }

        protected override void OnBeforeUpdate()
        {
            Scale = _mainCamera.orthographicSize;
        }

        protected override void SetColor(Color color)
        {
            _textMesh.color = color;
        }

        protected override void OnDispose() {}
        protected override void OnGameObjectDestroyed() {}
        protected override void UpdateLife() {}

        private TextMesh _textMesh;
        private UnityEngine.Camera _mainCamera;
    }
}
