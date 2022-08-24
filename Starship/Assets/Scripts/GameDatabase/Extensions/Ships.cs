using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace GameDatabase.Extensions
{
    public static class ShipExtensions
    {
        public static IEnumerable<ShipBuild> Available(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item => !item.NotAvailableInGame);
        }

        public static IEnumerable<ShipBuild> Playable(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item => !item.NotAvailableInGame && item.DifficultyClass == DifficultyClass.Default);
        }

        public static IEnumerable<ShipBuild> Flagships(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item => item.Ship.ShipCategory == ShipCategory.Flagship);
        }

        public static IEnumerable<ShipBuild> Titans(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item => item.Ship.SizeClass == SizeClass.Titan);
        }

        public static IEnumerable<ShipBuild> Common(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item => item.Ship.ShipCategory == ShipCategory.Common);
        }

        public static IEnumerable<ShipBuild> ShipsAndFlagships(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item =>
                item.Ship.ShipCategory == ShipCategory.Common ||
                item.Ship.ShipCategory == ShipCategory.Rare || item.Ship.ShipCategory == ShipCategory.Flagship);
        }

        public static IEnumerable<ShipBuild> AllShipsExceptSpecials(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item =>
                item.Ship.ShipCategory != ShipCategory.Special &&
                item.Ship.ShipCategory != ShipCategory.Starbase);
        }

        public static IEnumerable<ShipBuild> Rare(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item => item.Ship.ShipCategory == ShipCategory.Rare);
        }

        public static IEnumerable<ShipBuild> NormalShips(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item =>
                item.Ship.ShipCategory == ShipCategory.Common ||
                item.Ship.ShipCategory == ShipCategory.Rare);
        }

        public static IEnumerable<ShipBuild> OfFaction(this IEnumerable<ShipBuild> ships, Faction faction,
            int distance = 0)
        {
            return ships.Where(item =>
                faction == Faction.Undefined && !item.Ship.Faction.Hidden && item.Ship.Faction.WanderingShipsDistance <= distance ||
                item.Ship.Faction == faction);
        }

        public static IEnumerable<ShipBuild> OfFactionExplicit(this IEnumerable<ShipBuild> ships, Faction faction)
        {
            return ships.Where(item => item.Ship.Faction == faction);
        }

        public static IEnumerable<ShipBuild> Neutral(this IEnumerable<ShipBuild> ships)
        {
            return ships.Where(item => item.Ship.Faction == Faction.Neutral);
        }

        public static IEnumerable<ShipBuild> OfClass(this IEnumerable<ShipBuild> ships, DifficultyClass shipClassMin,
            DifficultyClass shipClassMax)
        {
            return ships.Where(item => item.DifficultyClass <= shipClassMax && item.DifficultyClass >= shipClassMin);
        }

        public static IEnumerable<ShipBuild> BestAvailableClass(this IEnumerable<ShipBuild> ships,
            DifficultyClass shipClassMax)
        {
            var bestClass = DifficultyClass.Default;
            foreach (var ship in ships)
                if (ship.DifficultyClass > bestClass && ship.DifficultyClass <= shipClassMax)
                    bestClass = ship.DifficultyClass;

            return ships.Where(item => item.DifficultyClass == bestClass);
        }

        public static IEnumerable<ShipBuild> OfSize(this IEnumerable<ShipBuild> ships, SizeClass minSize,
            SizeClass maxSize)
        {
            return ships.Where(item => item.Ship.SizeClass <= maxSize && item.Ship.SizeClass >= minSize);
        }

        public static IEnumerable<ShipBuild> LessOrEqualClass(this IEnumerable<ShipBuild> ships,
            DifficultyClass shipClass)
        {
            return ships.Where(item => item.DifficultyClass <= shipClass);
        }

        public static IEnumerable<ShipBuild> GreaterOrEqualClass(this IEnumerable<ShipBuild> ships,
            DifficultyClass shipClass)
        {
            return ships.Where(item => item.DifficultyClass >= shipClass);
        }

        public static IEnumerable<ShipBuild> LimitLevel(this IEnumerable<ShipBuild> ships, int distance)
        {
            var maxSize = Maths.Distance.ToShipSize(distance);
            return ships.Where(item =>
                item.Ship.Layout.CellCount <=
                Mathf.Max(maxSize, DatabaseStatistics.SmallestShipSize(item.Ship.Faction)));
        }

        public static IEnumerable<ShipBuild> LimitLevelAndClass(this IEnumerable<ShipBuild> ships, int distance)
        {
            var maxSize = Maths.Distance.ToShipSize(distance);
            var maxShipClass = Maths.Distance.MaxShipClass(distance);
            var minShipClass = Maths.Distance.MinShipClass(distance);
            return ships.Where(item =>
                item.Ship.Layout.CellCount <=
                Mathf.Max(maxSize, DatabaseStatistics.SmallestShipSize(item.Ship.Faction)) &&
                item.DifficultyClass <= maxShipClass && item.DifficultyClass >= minShipClass);
        }
    }
}
