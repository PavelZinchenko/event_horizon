using UnityEngine;

namespace GameObjects
{
    public sealed class Ship : MonoBehaviour, IGameObjectInterface
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public Vector2 Offset { get; set; }
        public Color Color { get; set; }
        public float Power { get; set; }
        public float Lifetime { get; set; }

        public static Ship Create(Texture2D texture, float zOrder = 1)
        {
            var gameObject = new GameObject("Ship");
            gameObject.transform.localPosition = new Vector3(0, 0, zOrder);
            var ship = gameObject.AddComponent<Ship>();
            gameObject.CreateMeshFilter().sharedMesh = SharedResources.Instance.SquareMesh;
            gameObject.CreateDefaultMaterial(texture, Color.white);
            gameObject.GameObjectInterface().Color = Color.white;
            return ship;
        }

        public void OnEnable()
        {
            Update();
        }

        public void OnDestroy()
        {
            gameObject.Cleanup();
        }

        public void Update()
        {
            gameObject.Move(Position);
            transform.localEulerAngles = new Vector3(0, 0, Rotation);
            transform.localScale = Vector3.one * Scale;
            var renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.material.color = Color;
        }

        private Ship()
        {
        }
    }
}
