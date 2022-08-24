using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
    public interface IModification
    {
        string GetDescription(ILocalization localization);
        ModificationQuality Quality { get; }
        void Apply(ref ShipEquipmentStats stats);
        void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition);
        void Apply(ref WeaponStatModifier statModifier);
        void Apply(ref DeviceStats device);
        void Apply(ref DroneBayStats droneBay);
    }
}
