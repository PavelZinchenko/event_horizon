using UnityEngine;

namespace Monobehaviours
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Flicker : MonoBehaviour
    {
        [SerializeField] private float AlphaMultiplier = 1.0f;
        [SerializeField] private float Frequency = 1.0f;
        [SerializeField] private float MinValue = 0.0f;

        public void Awake()
        {
            _baseColor = GetComponent<SpriteRenderer>().color;
        }

        public void Update()
        {
            var alpha = MinValue + (1f-MinValue)*0.5f*(1 + Mathf.Sin(Mathf.PI*Time.unscaledTime*Frequency));
            var color = _baseColor;
            color.a *= alpha*AlphaMultiplier;

            GetComponent<SpriteRenderer>().color = color;
        }

        private Color _baseColor;
    }
}
