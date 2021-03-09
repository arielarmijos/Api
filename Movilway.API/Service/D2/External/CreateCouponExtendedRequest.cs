using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class CreateCouponExtendedRequest
    {
        [MessageBodyMember(Name = "CreateCuponExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public CreateCouponExtendedRequestBody Request { set; get; }

        public CreateCouponExtendedRequest()
        {
            Request = new CreateCouponExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CreateCouponExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }
    }
}