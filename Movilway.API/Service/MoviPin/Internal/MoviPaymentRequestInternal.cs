using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.MoviPin.Internal
{
    public class MoviPaymentRequestInternal : ApiRequestInternal
    {
        public Decimal Amount { set; get; }
        public int Type { set; get; }
        public String CouponID { set; get; }
    }
}

