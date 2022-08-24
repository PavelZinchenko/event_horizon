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
        }

        public static implicit operator bool(SpriteId spriteId)
        {
            return !string.IsNullOrEmpty(spriteId._name);
        }

        public Type Category { get { return _type; } }
        public string Id { get { return _name; } }

        private readonly Type _type;
        private readonly string _name;

        public static readonly SpriteId Empty = new SpriteId();
    }
}
