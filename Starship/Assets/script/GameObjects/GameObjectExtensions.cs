using System.IO;
using UnityEngine;
using System.Linq;

public static class GameObjectExtensions
{
    public static IGameObjectInterface GameObjectInterface(this GameObject gameObject)
    {
        return (IGameObjectInterface)gameObject.GetComponents<MonoBehaviour>().FirstOrDefault(item => item is IGameObjectInterface);
    }

    public static void CreateDefaultMaterial(this GameObject gameObject, Texture2D texture, Color color)
    {
        if (_defaultMaterial == null)
        {
            _defaultMaterial = Resources.Load<Material>("Materials/Default");
            if (_defaultMaterial == null)
                throw new FileNotFoundException();
        }

        var renderer = gameObject.CreateMeshRenderer();
        GameObject.DestroyImmediate(renderer.material);
        renderer.material = _defaultMaterial;
        renderer.material.color = color;
        renderer.material.mainTexture = texture;
    }

    public static Material CreateMaterial(this GameObject gameObject, Material baseMaterial, Texture2D texture, Color color)
    {
        var material = new Material(baseMaterial);

        var renderer = gameObject.CreateMeshRenderer();
        GameObject.DestroyImmediate(renderer.material);
        renderer.material = material;
        renderer.material.color = color;
        renderer.material.mainTexture = texture;

        return material;
    }

    public static MeshRenderer CreateMeshRenderer(this GameObject gameObject)
    {
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        return meshRenderer;
    }

    public static MeshFilter CreateMeshFilter(this GameObject gameObject)
    {
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        return meshFilter;
    }

    public static Mesh GetMesh(this GameObject gameObject)
    {
        return gameObject.CreateMeshFilter().mesh;
    }

    public static void Move(this GameObject gameObject, Vector2 position)
    {
        gameObject.transform.localPosition = new Vector3(position.x, position.y, gameObject.transform.localPosition.z);
    }

    public static void Cleanup(this GameObject gameObject)
    {
        var renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            var materials = renderer.materials;
            foreach (var item in materials)
                GameObject.DestroyImmediate(item);
        }

        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            if (!meshFilter.sharedMesh)
                GameObject.Destroy(meshFilter.mesh);
        }
    }

    public static bool IsNull(this GameObject gameObject)
    {
        return object.ReferenceEquals(gameObject, null) || gameObject.GetHashCode() == 0;
    }

    private static Material _defaultMaterial;
}
