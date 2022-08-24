using GameDatabase.Model;
using GameDatabase.Serializable;

namespace GameDatabase.DataModel
{
    public partial class Faction
    {
        static Faction()
        {
            Neutral = new Faction(0, UnityEngine.Color.grey, "$NeutralFaction", 0, 0);
            DefaultValue = Undefined = new Faction(-1, UnityEngine.Color.black, "UNDEFINED", 0, 0);
        }

        private Faction(int id, UnityEngine.Color color, string name, int level, int shipLevel)
        {
            Id = new ItemId<Faction>(id);
            Color = color;
            Name = name;
            HomeStarDistance = level;
            WanderingShipsDistance = shipLevel;
        }

        public static readonly Faction Neutral;
        public static readonly Faction Undefined;
    }
}
