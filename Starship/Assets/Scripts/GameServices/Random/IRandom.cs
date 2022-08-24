namespace GameServices.Random
{
    public interface IRandom
    {
        int Seed { get; }
        System.Random CreateRandom(int seed = 0);
        int RandomInt(int index);
        int RandomInt(int index, int maxValue);
        int RandomInt(int index, int minValue, int maxValue);
    }
}
