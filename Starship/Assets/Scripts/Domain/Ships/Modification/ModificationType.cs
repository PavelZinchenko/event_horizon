using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Economy;
using GameDatabase;
using GameDatabase.Model;
using UnityEngine;

namespace Constructor.Ships.Modification
{
    public enum ModificationType
    {
        Empty          =-1,
        AutoTargeting  = 0,
        HeatDefense    = 1,
        KineticDefense = 2,
        EnergyDefense  = 3,
        ExtraBlueCells = 4,
        LightWeight    = 5,
        Infected       = 6,
    }

    public static class ModificationTypeExtension
    {
        public static bool IsSuitable(this ModificationType type, IShipModel ship)
        {
            if (type == ModificationType.Empty)
                return true;

            if (ship.Modifications.Any(item => item.Type == type))
                return false;

            switch (type)
            {
                case ModificationType.AutoTargeting:
                    return AutoTargetingModification.IsSuitable(ship.OriginalShip);
                default:
                    return true;
            }
        }

        public static Price GetInstallPrice(this ModificationType type)
        {
            if (type == ModificationType.Empty)
                return Price.Premium(0);
            else
                return Price.Premium(10);
        }

        public static SpriteId GetIconId(this ModificationType type)
        {
            switch (type)
            {
                case ModificationType.AutoTargeting:
                    return new SpriteId("icon_weapon", SpriteId.Type.GuiIcon);
                case ModificationType.Empty:
                    return new SpriteId(string.Empty, SpriteId.Type.GuiIcon);
                case ModificationType.EnergyDefense:
                    return new SpriteId("icon_energy_resist", SpriteId.Type.GuiIcon);
                case ModificationType.HeatDefense:
                    return new SpriteId("icon_fire_resist", SpriteId.Type.GuiIcon);
                case ModificationType.KineticDefense:
                    return new SpriteId("icon_impact_resist", SpriteId.Type.GuiIcon);
                case ModificationType.ExtraBlueCells:
                    return new SpriteId("icon_cargo", SpriteId.Type.GuiIcon);
                case ModificationType.LightWeight:
                    return new SpriteId("icon_gear", SpriteId.Type.GuiIcon);
                case ModificationType.Infected:
                    return new SpriteId("icon_virus", SpriteId.Type.GuiIcon);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Color GetColor(this ModificationType type)
        {
            switch (type)
            {
                case ModificationType.EnergyDefense:
                case ModificationType.HeatDefense:
                case ModificationType.KineticDefense:
                    return Color.white;
                case ModificationType.Empty:
                    return new Color(0,0,0,0);
                default:
                    return ColorTable.DefaultTextColor;
            }
        }
    }
}
