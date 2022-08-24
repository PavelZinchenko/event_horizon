using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OutlineEffect : EffectBase
    {
        [SerializeField] private float _effectSize;

        protected override void OnInitialize()
        {
            Scale = _effectSize;
            GetComponent<SpriteRenderer>().sprite = transform.parent.GetComponent<SpriteRenderer>().sprite; // TODO: temporary fix
        }

        protected override void OnDispose() {}
        protected override void OnGameObjectDestroyed() {}

        protected override void UpdateLife()
        {
            Opacity = Life;
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();

            var size = transform.lossyScale.z;
            GetComponent<SpriteRenderer>().material.SetFloat("_Thickness", Mathf.Min(5*(size + 10)/10, 10));
        }
    }
}
