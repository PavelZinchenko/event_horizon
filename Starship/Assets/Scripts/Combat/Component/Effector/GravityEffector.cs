using UnityEngine;

namespace Combat.Component.Effector
{
    [RequireComponent(typeof(PointEffector2D))]
    public class GravityEffector : MonoBehaviour, IEffector
    {
        public void Initialize(int collisionMask, float power)
        {
            Effector.colliderMask = collisionMask;
            Effector.forceMagnitude = -power;
        }

        private PointEffector2D Effector { get { return _effector ?? (_effector = GetComponent<PointEffector2D>()); } }

        private float _power;
        private PointEffector2D _effector;
    }
}
