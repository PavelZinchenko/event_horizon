using UnityEngine;

namespace Combat.Component.Physics.Joint
{
    public class Joint : IJoint
    {
        public Joint(Joint2D joint)
        {
            _joint = joint;
        }

        public bool IsActive { get { return _joint && _joint.connectedBody; } }

        public void Dispose()
        {
            if (_joint)
                Object.Destroy(_joint);
        }

        private readonly Joint2D _joint;
    }
}
