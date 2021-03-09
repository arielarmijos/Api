using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.D2.Internal
{
    public class CreateCouponResponseInternal:ApiResponseInternal
    {
        public String CouponID { set; get; }
        public Decimal Fee { set; get; }
        public String ResultNameSpace { set; get; }
        public long ScheduleID { set; get; }
        public String TransExtReference { set; get; }
    }
}