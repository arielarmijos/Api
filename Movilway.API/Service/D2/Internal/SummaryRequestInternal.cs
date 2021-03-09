using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.Internal
{
    public class SummaryRequestInternal : ApiRequestInternal
    {
        public int WalletType { set; get; }
        public String Target { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
    }
}