using GameDatabase.DataModel;

namespace Domain.Quests
{
    public class QuestContext
    {
        public QuestContext(QuestModel questModel, Galaxy.Star star, int seed)
        {
            QuestId = questModel.Id.Value;
            StarId = star.Id;
            Level = questModel.Level > 0 ? questModel.Level : star.Level;
            Faction = star.Region.Faction;
            Seed = seed;
        }

        public QuestContext(int seed)
        {
            StarId = 0;
            Seed = seed;
            Level = 0;
            Faction = Faction.Undefined;
        }

        public readonly int QuestId;
        public readonly int StarId;
        public readonly int Level;
        public readonly Faction Faction;
        public readonly int Seed;
    }
}
