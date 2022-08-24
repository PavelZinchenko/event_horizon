using System;

namespace Model
{
	namespace Regulations
	{
		public static class Market
		{
			public static long FuelRenewalTime { get { return TimeSpan.FromMinutes(1).Ticks; } }
			public static long RareComponentRenewalTime { get { return TimeSpan.FromDays(3).Ticks;; } }
			public static long CommonComponentRenewalTime { get { return TimeSpan.FromDays(1).Ticks; } }
			public static long ShipRenewalTime { get { return -1L; } }
			public static long StarsRenewalTime { get { return TimeSpan.FromDays(1).Ticks; } }
			public static long TechRenewalTime { get { return TimeSpan.FromHours(1).Ticks; } }
			public static long SatelliteRenewalTime { get { return TimeSpan.FromDays(3).Ticks; } }
			public static long GiftBoxRenewalTime { get { return TimeSpan.FromDays(1).Ticks; } }
		}
	}
}
