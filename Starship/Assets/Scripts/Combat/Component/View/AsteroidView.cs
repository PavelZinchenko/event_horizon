using UnityEngine;

namespace Combat.Component.View
{
    [RequireComponent(typeof(MeshFilter))]
    public class AsteroidView : BaseView
    {
        //[SerializeField] private Texture2D _texture;
        [SerializeField] private float _meshSize = 1.0f;
        //[SerializeField] private Color _materialColor = Color.white;
        [SerializeField] private Material _material;

        [SerializeField] private int _detailLevel = 6;
        [SerializeField] private float _noiseFrequency = 5f;
        [SerializeField] private float _noisePower = 0.2f;
        [SerializeField] private float _rotationSpeed = 1.0f;

        public override void Dispose()
        {
            _renderer = null;
        }

        public override void UpdateView(float elapsedTime)
        {
            _time += elapsedTime;
            transform.localEulerAngles = new Vector3(_rotationVector.x, _rotationVector.y, _rotationVector.z * _time);

            base.UpdateView(elapsedTime);
        }

        protected override void OnGameObjectCreated()
        {
            var meshFilter = GetComponent<MeshFilter>();
            //var renderer = GetComponent<Renderer>();

            _mesh = meshFilter.mesh;

            Primitives.CreateSphere(_mesh, _meshSize, _detailLevel, _noiseFrequency, _noisePower);

            //renderer.material.mainTexture = _texture;

            _rotationVector = new Vector3(Random.Range(0, 360), Random.Range(0, 360), _rotationSpeed * Random.Range(20.5f, 60f));

            //if (_material == null)
            //    gameObject.CreateDefaultMaterial(Texture, MaterialColor);
            //else
            //    gameObject.CreateMaterial(Material, Texture, MaterialColor);
        }

        protected override void OnGameObjectDestroyed()
        {
            foreach (var item in gameObject.GetComponent<Renderer>().materials)
                GameObject.DestroyImmediate(item);

            GameObject.Destroy(_mesh);
        }

        protected override void UpdateLife(float life) {}
        protected override void UpdatePosition(Vector2 position) {}
        protected override void UpdateRotation(float rotation) {}
        protected override void UpdateSize(float size) {}

        protected override void UpdateColor(Color color)
        {
            Renderer.material.color = color;
        }

        private MeshRenderer _renderer;
        // ReSharper disable once Unity.NoNullCoalescing
        private MeshRenderer Renderer => _renderer ?? (_renderer = GetComponent<MeshRenderer>());
        
        private float _time;
        private Vector3 _rotationVector;
        private Mesh _mesh;
    }
}
