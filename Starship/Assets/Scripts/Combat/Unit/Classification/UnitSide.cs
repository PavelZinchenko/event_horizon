namespace Combat.Component.Unit.Classification
{
    public enum UnitSide
    {
        Undefined = 0,
        Player = 1,
        Ally = 2,
        Enemy = 3,
        Neutral = 4,
    }

    public static class UnitSideExtensions
    {
        public static bool IsEnemy(this UnitSide first, UnitSide second)
        {
            if (first == second)
                return false;
            if (first == UnitSide.Player && second == UnitSide.Ally)
                return false;
            if (first == UnitSide.Ally && second == UnitSide.Player)
                return false;

            return true;
        }

        public static bool IsAlly(this UnitSide first, UnitSide second)
        {
            return !IsEnemy(first, second);
        }
    }
}
