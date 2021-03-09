using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.D2.Internal
{
    public class CreateCouponRequestInternal : ApiRequestInternal
    {
        public Decimal Amount { set; get; }
        public int CouponType { set; get; }
        public int WalletType { set; get; }
        public String Recipient { set; get; }
    }
}

