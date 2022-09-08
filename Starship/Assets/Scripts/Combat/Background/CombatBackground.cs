using UnityEngine;
using System.Collections.Generic;
using Services.Reources;
using Zenject;

namespace Combat.Background
{
    public class CombatBackground : MonoBehaviour
    {
        [SerializeField] private Color OutOfTimeColor = Color.red;
        [SerializeField] private float _size = 50f;
        [SerializeField] private Material _material;

        [Inject]
        void Initialize(IResourceLocator resourceLocator)
        {
            // Copy material to avoid modifying global material at runtime
            _material = _material != null ? new Material(_material) : null;
            _width = _height = _size * Screen.width / Screen.height;
            Create(_width, _height, 5);
            _material.mainTexture = resourceLocator.GetNebulaTexture(new System.Random().Next());
        }

        public bool OutOfTimeMode { get; set; }

        private void Update()
        {
            if ((object) _material == null) return;
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _material.mainTextureOffset = offset;

            offset *= 4;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _material.SetTextureOffset(DecalTex, offset);

            _material.color = Color.Lerp(_material.color, OutOfTimeMode ? OutOfTimeColor : Color.white, Time.deltaTime);
        }

        private void Create(float width, float height, int textureScale)
        {
            var mesh = gameObject.GetMesh();
            mesh.Clear(false);

            var vertices = new List<Vector3>();
            var uv = new List<Vector2>();
            var uv1 = new List<Vector2>();
            var triangles = new List<int>();

            for (var i = 0; i < textureScale; ++i)
            {
                for (var j = 0; j < textureScale; ++j)
                {
                    var x0 = (float)j / textureScale;
                    var y0 = (float)i / textureScale;
                    var x1 = (1.0f + j) / textureScale;
                    var y1 = (1.0f + i) / textureScale;

                    var index = vertices.Count;

                    vertices.Add(new Vector3(width * (x0 - 0.5f), height * (y1 - 0.5f), 0));
                    vertices.Add(new Vector3(width * (x1 - 0.5f), height * (y1 - 0.5f), 0));
                    vertices.Add(new Vector3(width * (x1 - 0.5f), height * (y0 - 0.5f), 0));
                    vertices.Add(new Vector3(width * (x0 - 0.5f), height * (y0 - 0.5f), 0));

                    uv.Add(new Vector2(x0, y1));
                    uv.Add(new Vector2(x1, y1));
                    uv.Add(new Vector2(x1, y0));
                    uv.Add(new Vector2(x0, y0));

                    uv1.Add(new Vector2(0, 1));
                    uv1.Add(new Vector2(1, 1));
                    uv1.Add(new Vector2(1, 0));
                    uv1.Add(new Vector2(0, 0));

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);
                    triangles.Add(index);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.uv2 = uv1.ToArray();

            mesh.RecalculateNormals();

            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _material;
        }

        private float _width;
        private float _height;
        private static readonly int DecalTex = Shader.PropertyToID("_DecalTex");
    }
}
