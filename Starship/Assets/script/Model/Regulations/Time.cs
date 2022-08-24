using System;

namespace Model
{
	namespace Regulations
	{
		public static class Time
		{
			public static bool IsCristmas
			{
				get 
				{
					/*var time = DateTime.Now;
					switch (time.Month)
					{
					case 12:
						return time.Day >= 18;
					case 1:
						return time.Day <= 9;
					default:
						return false;
					}*/
					return false;
				}
			}
		}
	}
}
