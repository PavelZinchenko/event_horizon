using System;
using UnityEngine;

namespace Combat.Component.View
{
    public class OrganicShipView : ShipView
    {
        public override void ApplyHsv(float hue, float saturation)
        {
            base.ApplyHsv(hue, saturation);

            _hue = hue;
            _saturation = saturation;
        }

        public override void UpdateView(float elapsedTime)
        {
            base.UpdateView(elapsedTime);
            if (!_hsvMaterialInstance) return;

            _time += elapsedTime * Mathf.PI;
            var pulse = Mathf.Pow(Mathf.Sin(_time), 8);

            var hue = _hue > 0.5f ? _hue - 0.1f * pulse : _hue + 0.1f * pulse;
            _hsvMaterialInstance.SetColor("_HSVAAdjust", new Color(hue, _saturation, 0));
        }

        private float _hue;
        private float _saturation;
        private float _time;
    }
}
