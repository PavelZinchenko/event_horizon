using UnityEngine;

namespace Combat.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShadowEffect : EffectBase
    {
        [SerializeField] private float _effectSize;
        [SerializeField] private Vector2 _offset;

        protected override void OnInitialize()
        {
            Scale = _effectSize;
            GetComponent<SpriteRenderer>().sprite = transform.parent.GetComponent<SpriteRenderer>().sprite; // TODO: temporary fix
        }

        protected override void OnDispose() { }
        protected override void OnGameObjectDestroyed() { }

        protected override void OnAfterUpdate()
        {
            transform.localPosition = Position + RotationHelpers.Transform(_offset, -transform.parent.localEulerAngles.z) / transform.parent.localScale.z;
        }

        protected override void UpdateLife()
        {
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();

            var size = transform.lossyScale.z;
            GetComponent<SpriteRenderer>().material.SetFloat("_Size", 0.1f / size);
        }
    }
}
