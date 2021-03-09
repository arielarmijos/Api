using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class BankListExtendedRequest
    {
        [MessageBodyMember(Name = "BankListExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public BankListExtendedRequestBody Request { set; get; }

        public BankListExtendedRequest()
        {
            Request = new BankListExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BankListExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String AgentReference { set; get; }
    }
}