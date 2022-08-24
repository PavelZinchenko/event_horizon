using GameDatabase.Enums;

namespace Model 
{
	//public enum DamageType
	//{
	//	Impact,
	//	Energy,
	//	Heat,
	//	Direct,
	//}

	public static class DamageTypeExtension
	{
		public static string Name(this DamageType type)
		{
			switch (type)
			{
			case DamageType.Impact:
				return "$ImpactDamage";
			case DamageType.Energy:
				return "$EnergyDamage";
			case DamageType.Heat:
				return "$HeatDamage";
			default:
				return "-";
			}
		}
	}
}
