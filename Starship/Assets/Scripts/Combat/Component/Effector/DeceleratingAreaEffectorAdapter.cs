using UnityEngine;

namespace Combat.Component.Effector
{
    [RequireComponent(typeof(AreaEffector2D))]
    public class DeceleratingAreaEffectorAdapter : MonoBehaviour, IEffector
    {
        public void Initialize(int collisionMask, float power)
        {
            Effector.colliderMask = collisionMask;
            Effector.drag = power;
            Effector.angularDrag = power;
        }

        private AreaEffector2D Effector { get { return _effector ?? (_effector = GetComponent<AreaEffector2D>()); } }

        private float _power;
        private AreaEffector2D _effector;
    }
}
