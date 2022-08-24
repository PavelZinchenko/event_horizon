using UnityEngine;

namespace Combat.Component.View
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshView : BaseView
    {
        [SerializeField] private Color _baseColor = Color.white;
        [SerializeField] private float _rotationSpeed = 1.0f;

        public override void Dispose() { }

        public override void UpdateView(float elapsedTime)
        {
            _time += elapsedTime;
            transform.localEulerAngles = new Vector3(_rotationVector.x, _rotationVector.y, _rotationVector.z * _time);

            base.UpdateView(elapsedTime);
        }

        protected override void OnGameObjectCreated()
        {
            _rotationVector = new Vector3(Random.Range(0, 360), Random.Range(0, 360), _rotationSpeed * Random.Range(20.5f, 60f));
        }

        protected override void OnGameObjectDestroyed()
        {
            foreach (var item in gameObject.GetComponent<Renderer>().materials)
                GameObject.DestroyImmediate(item);
        }

        protected override void UpdateLife(float life) { }
        protected override void UpdatePosition(Vector2 position) { }
        protected override void UpdateRotation(float rotation) { }
        protected override void UpdateSize(float size) { }

        protected override void UpdateColor(Color color)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = color * _baseColor;
        }

        private float _time;
        private Vector3 _rotationVector;
    }
}
