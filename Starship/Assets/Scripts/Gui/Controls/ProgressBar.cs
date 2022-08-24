using UnityEngine;
using UnityEngine.UI;

namespace Gui.Controls
{
    [AddComponentMenu("UI/ProgressBar", 1000)]
    public class ProgressBar : Graphic
    {
        [SerializeField] private Sprite _image;
        [SerializeField] private float _aspectRatio = 1.0f;

        [Range(0.0f, 1.0f)] public float X0 = 0;
        [Range(0.0f, 1.0f)] public float X1 = 1;
        [Range(0.0f, 1.0f)] public float Y0 = 0;
        [Range(0.0f, 1.0f)] public float Y1 = 1;

        public override Texture mainTexture { get { return _image != null ? _image.texture : base.mainTexture; } }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            var rect = rectTransform.rect;

            var x0 = rect.xMin + X0*rect.width;
            var x1 = rect.xMin + X1*rect.width;
            var y0 = rect.yMax - Y0*rect.height;
            var y1 = rect.yMax - Y1*rect.height;
            
            var u = _image == null ? 1 : _aspectRatio*rect.width / _image.pixelsPerUnit;
            var v = _image == null ? 1 : _aspectRatio*rect.height / _image.pixelsPerUnit;

            var u0 = X0*u;
            var u1 = X1*u;
            var v0 = Y0*v;
            var v1 = Y1*v;

            vertexHelper.Clear();
            vertexHelper.AddVert(new Vector2(x0, y0), color, new Vector2(u0, v0));
            vertexHelper.AddVert(new Vector2(x1, y0), color, new Vector2(u1, v0));
            vertexHelper.AddVert(new Vector2(x1, y1), color, new Vector2(u1, v1));
            vertexHelper.AddVert(new Vector2(x0, y1), color, new Vector2(u0, v1));
            vertexHelper.AddTriangle(0, 1, 2);
            vertexHelper.AddTriangle(2, 3, 0);
        }
    }
}
