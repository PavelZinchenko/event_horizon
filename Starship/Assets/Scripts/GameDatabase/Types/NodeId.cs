using System;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace GameDatabase.Model
{
    public struct NodeId : IEquatable<NodeId>
    {
        public NodeId(int id)
        {
            _id = UnityEngine.Mathf.Clamp(id, 0, 1000);
        }

        public static implicit operator NodeId(int value)
        {
            return new NodeId(value);
        }

        public static implicit operator int(NodeId value)
        {
            return value._id;
        }

        public bool Equals(NodeId other)
        {
            return _id == other._id;
        }

        public static bool operator ==(NodeId c1, NodeId c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(NodeId c1, NodeId c2)
        {
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NodeId)) return false;
            return Equals((NodeId)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        private readonly int _id;
    }
}
