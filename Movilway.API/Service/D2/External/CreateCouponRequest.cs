using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class CreateCouponRequest
    {
        [MessageBodyMember(Name = "CreateCuponRequest", Namespace = "http://api.movilway.net/schema")]
        public CreateCouponRequestBody Request { set; get; }

        public CreateCouponRequest()
        {
            Request = new CreateCouponRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CreateCouponRequestBody:SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }
    }
}