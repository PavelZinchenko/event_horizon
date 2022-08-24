using Model.Military;

namespace GameModel.Achievements
{
    public class BaptismByFire : IAchievement
    {
        public BaptismByFire(bool completed)
        {
            if (completed)
            {
                Completed = true;
                return;
            }

            // TODO: ServiceLocator.Messenger.AddListener<CombatResult>(EventType.CombatCompleted, OnCombatCompleted, SubscribtionLifeTime.Session);
        }

        public AchievementType Type { get { return AchievementType.BaptismByFire; } }

        public bool Completed { get; private set; }

        private void Complete()
        {
            Completed = true;
            // TODO: ServiceLocator.Messenger.RemoveListener<CombatResult>(EventType.CombatCompleted, OnCombatCompleted, SubscribtionLifeTime.Session);
            // TODO: ServiceLocator.Messenger.Broadcast<IAchievement>(EventType.AchievementUnlocked, this);
        }
    }
}
