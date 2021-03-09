using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class SellExtendedRequest
    {
        [MessageBodyMember(Name = "SellExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public SellExtendedRequestBody Request { set; get; }

        public SellExtendedRequest()
        {
            Request = new SellExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SellExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String Agent { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public int Type { set; get; }
    }
}