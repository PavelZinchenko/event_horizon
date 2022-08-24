namespace GameModel.Achievements
{
    public interface IAchievement
    {
        AchievementType Type { get; }
        bool Completed { get; }
    }
}
