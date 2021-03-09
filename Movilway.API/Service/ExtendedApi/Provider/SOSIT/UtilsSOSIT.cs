using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.ExtendedApi.Provider.SOSIT
{
    public static class UtilsSOSIT    
    {
        internal static DateTime GetLocalTimeZone(int countryId = 0)
        {

            if (countryId == 0)
                countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryID"]);
            
            int[] _utc_AR = new int[] { 1, 14 };//SA Pacific Standard Time
            int[] _utc_m5 = new int[] { 3, 7, 4, 6, 17 };//SA Pacific Standard Time
            int[] _utc_m6 = new int[] { 10, 11, 12 };//Central America Standard Time

            if (_utc_AR.Contains(countryId))//Argentina
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"));
            else if (countryId == 2)//Uruguay
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Montevideo Standard Time"));
            else if (_utc_m5.Contains(countryId))
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
            else if (countryId == 5)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
            else if (countryId == 8)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
            else if (countryId == 9)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
            else if (_utc_m6.Contains(countryId))
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time"));
            else if (countryId == 13)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time"));
            else if (countryId == 15)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            else if (countryId == 16)
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time"));

            else return DateTime.Now;
        }


        internal static DateTime GetLocalTimeZoneFromUtc( DateTime dateUtc, int countryId = 0)
        {
            if (countryId == 0)
                countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryID"]);

            int[] _utc_AR = new int[] { 1, 14 };//SA Pacific Standard Time
            int[] _utc_m5 = new int[] { 3, 7, 4, 6, 17 };//SA Pacific Standard Time
            int[] _utc_m6 = new int[] { 10, 11, 12 };//Central America Standard Time

            if (_utc_AR.Contains(countryId))//Argentina
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"));
            else if (countryId == 2)//Uruguay
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("Montevideo Standard Time"));
            else if (_utc_m5.Contains(countryId))
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
            else if (countryId == 5)
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
            else if (countryId == 8)
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
            else if (countryId == 9)
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
            else if (_utc_m6.Contains(countryId))
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time"));
            else if (countryId == 13)
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time"));
            else if (countryId == 15)
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            else if (countryId == 16)
                return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time"));

            else return DateTime.Now;
        }

        internal static DateTime FromUtcToLocalTime(string ParamTimeStamp)
        {
            DateTime dateTime1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToDouble(ParamTimeStamp));
            DateTime dateTimeNew = GetLocalTimeZoneFromUtc(dateTime1, 0);
            return (dateTimeNew);
        }
    }
}