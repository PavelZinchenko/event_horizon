using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LayeredEffect : EffectBase
    {
        [SerializeField] private SpriteRenderer[] _children;

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            var random = new System.Random(_seed);
            foreach (var child in _children)
            {
                var rotation = Rotation*(2*random.NextFloat() - 1) + random.Next(360);
                child.transform.localEulerAngles = new Vector3(0,0,rotation);
            }
        }

        protected override void SetColor(Color color)
        {
            base.SetColor(color);
            var random = new System.Random(_seed);

            foreach (var child in _children)
                child.color = new Color(
                    color.r*(random.NextFloat() * 0.2f + 0.9f), 
                    color.g*(random.NextFloat() * 0.2f + 0.9f),
                    color.b*(random.NextFloat() * 0.2f + 0.9f),
                    color.a*(random.NextFloat() * 0.2f + 0.9f));
        }

        protected override void OnInitialize()
        {
            var random = new System.Random();
            _seed = random.Next();

            foreach (var child in _children)
            {
                child.transform.localScale = Vector3.one * (random.NextFloat()*0.4f + 0.8f);
                child.gameObject.Move(new Vector2(random.NextFloat()*0.4f - 0.2f, random.NextFloat()*0.4f - 0.2f));
            }
        }

        protected override void OnDispose() {}
        protected override void OnGameObjectDestroyed() {}

        protected override void UpdateLife()
        {
            Opacity = 1.0f - Mathf.Pow(1.0f - Life, 4f);
        }

        private int _seed;
    }
}
