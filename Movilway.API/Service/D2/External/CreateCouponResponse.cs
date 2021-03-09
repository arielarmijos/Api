using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{
    [MessageContract(IsWrapped = false)]
    public class CreateCouponResponse
    {
        [MessageBodyMember(Name = "CreateCuponResponse", Namespace = "http://api.movilway.net/schema")]
        public CreateCouponResponseBody Response { set; get; }

        public CreateCouponResponse()
        {
            Response = new CreateCouponResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CreateCouponResponseBody : ApiResponse
    {
        [DataMember(Order = 0)]
        public String CouponID { set; get; }

        [DataMember(Order = 1)]
        public Decimal Fee { set; get; }

        [DataMember(Order = 2)]
        public String ResultNameSpace{ set; get; }

        [DataMember(Order = 3)]
        public long ScheduleID { set; get; }

        [DataMember(Order = 4)]
        public String TransExtReference { set; get; }
    }
}