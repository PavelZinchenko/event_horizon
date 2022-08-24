using System.Collections.Generic;
using System.Text;
using Constructor.Model;
using GameDatabase.Enums;
using GameDatabase.Model;
using Services.Localization;

namespace Constructor.Ships.Modification
{
    public class ExtraCellsModification : IShipModification
    {
        public ExtraCellsModification(int seed)
        {
            Seed = seed;
        }

        public ModificationType Type => ModificationType.ExtraBlueCells;

        public string GetDescription(ILocalization localization)
        {
            return localization.GetString("$Ship_ExtraBlueCells");
        }

        public void Apply(ref ShipBaseStats stats)
        {
            var size = stats.Layout.Size;
            var random = new System.Random(Seed);
            var suitableCells = new List<int>();
            var cellCount = 0;

            var layout = stats.Layout.Data;
            for (var i = 0; i < layout.Length; ++i)
            {
                var x = i % size;
                var y = i / size;

                var cell = (CellType)layout[i];
                if (cell != CellType.Empty)
                {
                    cellCount++;
                    continue;
                }

                if (y > 1 && IsSuitableCell(layout[i - size]))
                    suitableCells.Add(i);
                else if (y < size - 1 && IsSuitableCell(layout[i + size]))
                    suitableCells.Add(i);
                else if (x > 1 && IsSuitableCell(layout[i - 1]))
                    suitableCells.Add(i);
                else if (x < size - 1 && IsSuitableCell(layout[i + 1]))
                    suitableCells.Add(i);
            }

            var result = new StringBuilder(layout);
            foreach (var cell in suitableCells.RandomUniqueElements(1 + cellCount / 20, random))
                result[cell] = (char)CellType.Outer;

            stats.Layout = new Layout(result.ToString());
        }

        public int Seed { get; }

        private static bool IsSuitableCell(char cell) { return cell == (char)CellType.Inner || cell == (char)CellType.Outer || cell == (char)CellType.InnerOuter; }
    }
}
