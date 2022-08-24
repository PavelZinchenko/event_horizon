using UnityEngine;

namespace GameDatabase.Model
{
    public struct ImageData
    {
        public ImageData(byte[] data)
        {
            var texture = new Texture2D(2, 2);
            if (texture.LoadImage(data) && texture.width == texture.height)
                Sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
            else
                Sprite = null;
        }

        public Sprite Sprite { get; }

        public static ImageData Empty = new ImageData();
    }
}
