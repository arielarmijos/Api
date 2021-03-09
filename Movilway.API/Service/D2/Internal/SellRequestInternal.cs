using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.Internal
{
    public class SellRequestInternal : ApiRequestInternal
    {
        public Decimal Amount { set; get; }
        public String Agent { set; get; }
        public int Type { set; get; }
    }
}