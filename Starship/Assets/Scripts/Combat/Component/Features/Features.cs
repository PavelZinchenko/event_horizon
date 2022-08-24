using Combat.Component.Collider;
using Combat.Component.Mods;
using Combat.Component.View;
using UnityEngine;

namespace Combat.Component.Features
{
    public class Features : IFeatures
    {
        public Features(TargetPriority targetPriority, Color color)
        {
            _targetPriority = targetPriority;
            _color = color;
            UpdateData();
        }

        public TargetPriority TargetPriority { get { return _data.TargetPriority; } }

        public Color Color
        {
            get
            {
                var color = _data.Color;
                color.a *= _data.Opacity;
                return color;
            }
        }

        public void UpdatePhysics(float elapsedTime, ICollider collider)
        {
            UpdateData();
            collider.Enabled = !_data.Invulnerable;
        }

        public void UpdateView(float elapsedTime, IView view)
        {
            view.Color = Color;
        }

        public Modifications<FeaturesData> Modifications { get { return _modifications; } }

        public void Dispose() {}

        private void UpdateData()
        {
            _data.Color = _color;
            _data.Opacity = 1.0f;
            _data.TargetPriority = _targetPriority;
            _data.Invulnerable = false;

            _modifications.Apply(ref _data);
        }

        private FeaturesData _data;
        private readonly TargetPriority _targetPriority;
        private readonly Color _color;
        private readonly Modifications<FeaturesData> _modifications = new Modifications<FeaturesData>();
    }
}
