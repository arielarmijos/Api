using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class CashInExtendedRequest
    {
        [MessageBodyMember(Name = "CashInExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public CashInExtendedRequestBody Request { set; get; }

        public CashInExtendedRequest()
        {
            Request = new CashInExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class CashInExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public String Recipient { set; get; }
    }
}