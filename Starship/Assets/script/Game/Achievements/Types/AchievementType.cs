using System.Collections.Generic;
using System.ComponentModel;
using Economy.ItemType;
using Economy.Products;

namespace GameModel.Achievements
{
    public enum AchievementType
    {
        BaptismByFire,
        //Voyager100,
        //Voyager1000,
        //Voyager10000,
    }

    public static class AchievementTypeExtensions
    {
        public static IAchievement Create(this AchievementType type, bool completed)
        {
            switch (type)
            {
                case AchievementType.BaptismByFire:
                    return new BaptismByFire(completed);
                default:
                    throw new InvalidEnumArgumentException("type", (int)type, typeof(AchievementType));
            }
        }

        public static IEnumerable<IProduct> GetReward(this AchievementType type, ItemTypeFactory factory)
        {
            switch (type)
            {
                case AchievementType.BaptismByFire:
                    yield return Economy.Price.Premium(1).GetProduct(factory);
                    break;
                default:
                    yield return Economy.Price.Premium(5).GetProduct(factory);
                    break;
            }
        }
    }
}
