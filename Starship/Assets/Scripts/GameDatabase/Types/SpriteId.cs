using System.Collections.Generic;

namespace GameDatabase.Model
{
    public struct SpriteId
    {
        public enum Type
        {
            Default,
            Ship,
            ShipIcon,
            Component,
            Satellite,
            ActionButton,
            GuiIcon,
            SkillIcon,
            AvatarIcon,
            ArtifactIcon,
            Ammunition,
            Effect,
        }

        public SpriteId(string name, Type type)
        {
            _name = name;
            _type = type;
            if (name == null) _id = -1;
            else if (!NameIdMappings.TryGetValue(name, out _id))
            {
                _id = NameIdMappings[name] = lastId++;
            }
        }

        public static implicit operator bool(SpriteId spriteId)
        {
            return !string.IsNullOrEmpty(spriteId._name);
        }

        public Type Category => _type;

        public string Id => _name;

        public int NumericalId => _id;

        private readonly Type _type;
        private readonly string _name;
        private readonly int _id;

        public static readonly Dictionary<string, int> NameIdMappings = new Dictionary<string, int>();
        private static int lastId = 1;
        public static readonly SpriteId Empty = new SpriteId();
    }
}
