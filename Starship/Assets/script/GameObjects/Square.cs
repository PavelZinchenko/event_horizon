using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour
{
    public float Size = 1.0f;
    public Color Color = Color.white;
    public Texture2D Texture;
    public Material Material;

    public void SetColor(Color color)
    {
        if (_renderer == null)
            _renderer = gameObject.GetComponent<Renderer>();
        if (_renderer != null)
            _renderer.material.color = color;
    }

    void Awake()
    {
        gameObject.transform.localScale = Vector3.one * Size;
        gameObject.CreateMeshFilter().sharedMesh = SharedResources.Instance.SquareMesh;
        if (Material != null)
            gameObject.CreateMaterial(Material, Texture, Color);
        else
            gameObject.CreateDefaultMaterial(Texture, Color);
    }

    private Renderer _renderer;
}
