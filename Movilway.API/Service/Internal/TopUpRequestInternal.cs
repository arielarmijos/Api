using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.Internal
{
    public class TopUpRequestInternal : ApiRequestInternal
    {
        public String MNO { set; get; }
        public Decimal Amount { set; get; }
        public String Recipient { set; get; }
        public String HostTransRef { set; get; }
        public String MNODefinedID { set; get; }
    }
}