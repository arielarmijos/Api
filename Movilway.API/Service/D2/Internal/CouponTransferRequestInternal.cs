using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.D2.Internal
{
    public class CouponTransferRequestInternal : ApiRequestInternal
    {
        public Decimal Amount { set; get; }
        public int Type { set; get; }
        public String CouponID { set; get; }
    }
}

