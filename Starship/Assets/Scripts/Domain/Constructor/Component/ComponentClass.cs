using Constructor.Component;
using GameDatabase.Enums;

namespace Constructor
{
	public static class ComponentExtension
	{
	    public static IComponent Create(this GameDatabase.DataModel.Component component, int shipSize)
	    {
	        return new CommonComponent(component, shipSize);
	    }

        public static string GetUniqueKey(this GameDatabase.DataModel.Component component)
        {
            if (!string.IsNullOrEmpty(component.Restrictions.UniqueComponentTag))
                return component.Restrictions.UniqueComponentTag;

            if (component.Device != null)
            {
                switch (component.Device.Stats.DeviceClass)
                {
                    case DeviceClass.Teleporter:
                    case DeviceClass.Fortification:
                    case DeviceClass.Brake:
                    case DeviceClass.RepairBot:
                    case DeviceClass.PointDefense:
                    case DeviceClass.GravityGenerator:
                    case DeviceClass.Ghost:
                    case DeviceClass.Decoy:
                    case DeviceClass.Detonator:
                    case DeviceClass.Accelerator:
                    case DeviceClass.ToxicWaste:
                        return component.Device.Stats.DeviceClass.ToString();
                    case DeviceClass.Stealth:
                    case DeviceClass.SuperStealth:
                        return DeviceClass.Stealth.ToString();
                    case DeviceClass.EnergyShield:
                    case DeviceClass.PartialShield:
                        return DeviceClass.EnergyShield.ToString();
                    case DeviceClass.WormTail:
                        return DeviceClass.WormTail.ToString();
                    default:
                        return null;
                }
            }

            return null;
        }
    }
}
