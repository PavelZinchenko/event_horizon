using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Modification
{
	class BulletVelocity2 : IModification
	{
		public BulletVelocity2(ModificationQuality quality)
		{
			_velocityMultiplier = quality.PowerMultiplier(0.7f, 0.8f, 0.9f, 1.2f, 1.5f, 2.0f);
			_damageMultiplier = quality.PowerMultiplier(0.8f, 0.85f, 0.9f, 0.9f, 0.8f, 0.75f);
			Quality = quality;
		}

		public string GetDescription(ILocalization localization)
        {
			return localization.GetString(
				"$BulletVelocityMod2",
				Maths.Format.SignedPercent(_velocityMultiplier - 1.0f),
				Maths.Format.SignedPercent(_damageMultiplier - 1.0f)); 
		}

		public ModificationQuality Quality { get; private set; }

        public void Apply(ref ShipEquipmentStats stats) { }
        public void Apply(ref DeviceStats device) { }
        public void Apply(ref DroneBayStats droneBay) { }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition)
        {
            ammunition.Velocity *= _velocityMultiplier;
            ammunition.Damage *= _damageMultiplier;
        }

	    public void Apply(ref WeaponStatModifier statModifier)
	    {
	        statModifier.VelocityMultiplier *= _velocityMultiplier;
	        statModifier.DamageMultiplier *= _damageMultiplier;
	    }

        private readonly float _velocityMultiplier;
		private readonly float _damageMultiplier;
	}
}
