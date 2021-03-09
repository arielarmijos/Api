using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.Internal
{
    public class BuyRequestInternal : ApiRequestInternal
    {
        public String Target { set; get; }
        public Decimal Amount { set; get; }
        public String Recipient { set; get; }
    }
}