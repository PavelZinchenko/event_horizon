using UnityEngine;

namespace GameDatabase.Model
{
    public class ImageData
    {
        public static ImageData Empty = new ImageData(null);
        private readonly byte[] bytes;
        private bool gotValue;
        private Sprite sprite;

        public ImageData(byte[] data)
        {
            bytes = data;
            sprite = null;
            gotValue = data == null;
        }

        public Sprite Sprite
        {
            get
            {
                if (gotValue) return sprite;
                var texture = new Texture2D(2, 2);
                if (texture.LoadImage(bytes) && texture.width == texture.height)
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        texture.width);
                else sprite = null;
                gotValue = true;
                return sprite;
            }
        }
    }
}
