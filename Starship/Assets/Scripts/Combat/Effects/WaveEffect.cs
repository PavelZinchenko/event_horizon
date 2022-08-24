using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(MeshFilter))]
    public class WaveEffect : EffectBase
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private float _thickness = 1.0f;
        [SerializeField] private float _radius = 5.0f;
        [SerializeField] private int _segments = 32;
        [SerializeField] private float _baseSize = 1.0f;

        public void SetSize(float value) { _baseSize = value; }

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                if (_material) _material.mainTexture = _texture;
            }
        }

        protected override void OnInitialize()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var renderer = GetComponent<Renderer>();

            _mesh = meshFilter.mesh;
            _material = renderer.material;
            _material.mainTexture = _texture;
            Primitives.CreateCircle(_mesh, _radius, _thickness, _segments, (float)_texture.width / (float)_texture.height);
        }

        protected override void OnDispose() {}

        protected override void OnGameObjectDestroyed()
        {
            Destroy(_material);
            Destroy(_mesh);
        }

        protected override void UpdateLife()
        {
            Opacity = Life;
            Scale = (1f - Life)* _baseSize;
        }

        private Mesh _mesh;
        private Material _material;
    }
}
