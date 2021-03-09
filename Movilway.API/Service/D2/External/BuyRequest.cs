using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class BuyRequest
    {
        [MessageBodyMember(Name = "BuyRequest", Namespace = "http://api.movilway.net/schema")]
        public BuyRequestBody Request { set; get; }

        public BuyRequest()
        {
            Request = new BuyRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BuyRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Target { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 4, IsRequired = true)]
        public String Recipient { set; get; }
    }
}