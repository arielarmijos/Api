using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Movilway.API.Service.ExtendedApi.DataContract.Common
{
    public class UtibaUtils
    {
        static UtibaUtils()
        {
            SessionTTL = int.Parse(ConfigurationManager.AppSettings["UtibaSessionTTL"] ?? "5");
        }

        public static  DateTime FromEpochToLocalTime(long epochTimeStamp)
        {
            DateTime newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            newDateTime = newDateTime.AddMilliseconds(epochTimeStamp);
            newDateTime = newDateTime.ToLocalTime();
            return (newDateTime);
        }

        public static int FromDateTimeToEpoch(DateTime normalDateTime)
        {
            TimeSpan t = (normalDateTime.ToUniversalTime() - new DateTime(1970, 1, 1));
            return (int)t.TotalSeconds;
        }

        public static int SessionTTL { get; set; }
    }
}