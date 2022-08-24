using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class DateTimeExtensions
    {
        public static bool IsEaster(this System.DateTime dateTime)
        {
            if (dateTime.Year == 2017)
                return dateTime.Month == 4 && dateTime.Day > 9 && dateTime.Day < 23;
            return false;
        }
    }
}
