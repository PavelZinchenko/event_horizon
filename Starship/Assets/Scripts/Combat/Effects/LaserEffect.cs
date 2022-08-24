using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(MeshFilter))]
    public class LaserEffect : EffectBase
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private float _thickness = 1.0f;
        [SerializeField] private float _borderSize = 0.2f;

        protected override void OnInitialize()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var renderer = GetComponent<Renderer>();

            _mesh = meshFilter.mesh;
            renderer.material.mainTexture = _texture;
        }

        protected override void OnDispose() {}

        protected override void OnGameObjectDestroyed()
        {
            GameObject.Destroy(_mesh);
        }

        protected override void UpdateSize()
        {
            var size = Size*Scale;
            if (size > 2*_borderSize)
                Primitives.CreateLine(_mesh, Size*Scale, _thickness, _borderSize);
            else
                _mesh.Clear();
        }

        protected override void UpdateLife()
        {
            Opacity = Life;
        }

        private Mesh _mesh;
    }
}
