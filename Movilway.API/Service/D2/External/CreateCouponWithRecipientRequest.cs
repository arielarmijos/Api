using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{
    [MessageContract(IsWrapped = false)]
    public class CreateCouponWithRecipientRequest
    {
        [MessageBodyMember(Name = "CreateCouponWithRecipientRequest", Namespace = "http://api.movilway.net/schema")]
        public CreateCouponWithRecipientRequestBody Request { set; get; }

        public CreateCouponWithRecipientRequest()
        {
            Request = new CreateCouponWithRecipientRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CreateCouponWithRecipientRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String Recipient { set; get; }

    }
}