using System;
using System.Collections.Generic;
using Combat.Component.Physics.Joint;
using UnityEngine;

namespace Combat.Component.Physics
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsManager : MonoBehaviour, IDisposable
    {
        public IJoint CreateDistanceJoint(PhysicsManager other, float maxDistance)
        {
            if (other == null)
                return null;

            var joint = gameObject.AddComponent<DistanceJoint2D>();
            joint.connectedBody = other.Rigidbody;
            joint.maxDistanceOnly = true;
            joint.enableCollision = true;
            joint.distance = maxDistance;

            _joints.Add(joint);
            return new Joint.Joint(joint);
        }

        public IJoint CreateHingeJoint(PhysicsManager other, float offset, float connectedOffset, float minAngle, float maxAngle)
        {
            if (other == null)
                return null;

            var joint = gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = other.Rigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = new Vector2(-offset, 0f);
            joint.connectedAnchor = new Vector2(connectedOffset,0f);
            joint.useLimits = true;
            joint.limits = new JointAngleLimits2D {min = minAngle, max = maxAngle};

            _joints.Add(joint);
            return new Joint.Joint(joint);
        }

        public IJoint CreateFixedJoint(PhysicsManager other, Vector2 position)
        {
            if (other == null)
                return null;

            var joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = other.Rigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector2.zero;
            joint.connectedAnchor = position;
            joint.dampingRatio = 1.0f;

            _joints.Add(joint);
            return new Joint.Joint(joint);
        }

        public void Dispose()
        {
            foreach (var item in _joints)
                GameObject.Destroy(item);
            _joints.Clear();
        }

        protected Rigidbody2D Rigidbody { get { return _rigidbody ?? (_rigidbody = GetComponent<Rigidbody2D>()); } }

        protected Rigidbody2D _rigidbody;
        private List<Joint2D> _joints = new List<Joint2D>();
    }
}
