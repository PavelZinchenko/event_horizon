namespace Combat.Component.Mods
{
    public interface IModification<T> where T : struct
    {
        bool TryApplyModification(ref T data);
    }
}
