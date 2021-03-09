using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Log;

namespace Movilway.API.Service.Internal
{
    public class LoginRequestInternal
    {
        public String User { set; get; }
        [NotLoggable]
        public String Password { set; get; }
        public int DeviceType { set; get; }
    }
        
}