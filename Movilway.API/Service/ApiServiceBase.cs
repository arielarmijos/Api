using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.ServiceModel;

namespace Movilway.API.Service
{
    public abstract class ApiServiceBase
    {
        public static String UserAgent { get{ return(_userAgent);} }
        
        private static String _userAgent;
        private static Logger _logger;

        protected ApiServiceBase()
        {
            _userAgent = ConfigurationManager.AppSettings["UserAgent"];
            _logger = new Logger();
        }

        public static void Log(Movilway.API.Service.Logger.LogMessageType logType, string message, Movilway.API.Service.Logger.LoggingLevelType loggingLevel)
        {
            _logger.Message(logType, message, loggingLevel);
        }

        public DateTime FromEpochToLocalTime(long epochTimeStamp)
        {
            var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            newDateTime = newDateTime.AddMilliseconds(epochTimeStamp);
            newDateTime = newDateTime.ToLocalTime();
            return (newDateTime);
        }

        public int FromDateTimeToEpoch(DateTime normalDateTime)
        {
            TimeSpan t = (normalDateTime.ToUniversalTime() - new DateTime(1970, 1, 1));
            return (int)t.TotalSeconds;
        }
    }
}