using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service
{
    public abstract class ApiRequestInternal
    {
        public String SessionID { set; get; }
        public int DeviceType { set; get; }
    }
}