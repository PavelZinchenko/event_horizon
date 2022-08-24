using System;

namespace GameDatabase.Model
{
    public struct PrefabId
    {
        public enum Type
        {
            Undefined,
            Bullet,
            Object,
            Effect,
        }

        public PrefabId(string name, Type type)
        {
            _name = name;
            _type = type;
        }

        public static implicit operator bool(PrefabId prefabId)
        {
            return !string.IsNullOrEmpty(prefabId._name);
        }

        public override string ToString()
        {
            switch (_type)
            {
                case Type.Undefined:
                    return _name;
                case Type.Bullet:
                    return "Combat/Bullets/" + _name;
                case Type.Object:
                    return "Combat/Objects/" + _name;
                case Type.Effect:
                    return "Combat/Effects/" + _name;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private readonly Type _type;
        private readonly string _name;

        public static readonly PrefabId Empty = new PrefabId(string.Empty, Type.Undefined);
    }
}
