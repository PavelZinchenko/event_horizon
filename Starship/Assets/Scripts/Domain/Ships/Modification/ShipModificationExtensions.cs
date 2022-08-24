using System;
using System.Collections.Generic;
using System.Linq;

namespace Constructor.Ships.Modification
{
    public static class ShipModificationExtensions
    {
        public static long Serialize(this IShipModification modification)
        {
            var type = (long)(uint)(modification.Type);
            var seed = (long)(uint)(modification.Seed);

            return (type << 32) | seed;
        }

        public static IShipModification Deserialize(long data, ModificationFactory factory)
        {
            var seed = (int)(uint)(data & 0xffffffff);
            var type = (ModificationType)(uint)(data >> 32);

            return factory.Create(type, seed);
        }

        public static IEnumerable<IShipModification> CreateRandomModifications(IShipModel model, int count, System.Random random, ModificationFactory factory)
        {
            return Enum.GetValues(typeof(ModificationType)).Cast<ModificationType>().Where(item => item.IsSuitable(model)).RandomUniqueElements(count, random).Select(item => factory.Create(item, random.Next()));
        }
    }
}
