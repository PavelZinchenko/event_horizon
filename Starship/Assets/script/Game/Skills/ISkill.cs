namespace GameModel.Skills
{
    public interface ISkill
    {
        string Name { get; }
        string Description { get; }
        UnityEngine.Sprite Icon { get; }
    }
}
