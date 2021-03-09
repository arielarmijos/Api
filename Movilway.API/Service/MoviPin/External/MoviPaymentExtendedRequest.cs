using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.MoviPin.External
{

    [MessageContract(IsWrapped = false)]
    public class MoviPaymentExtendedRequest
    {
        [MessageBodyMember(Name = "MoviPaymentExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public MoviPaymentExtendedRequestBody Request { set; get; }

        public MoviPaymentExtendedRequest()
        {
            Request = new MoviPaymentExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class MoviPaymentExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public int Type { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public String CouponID { set; get; }
    }
}