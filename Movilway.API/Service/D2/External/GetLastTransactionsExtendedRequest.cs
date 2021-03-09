using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class GetLastTransactionsExtendedRequest
    {
        [MessageBodyMember(Name = "GetLastTransactionsExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public GetLastTransactionsExtendedRequestBody Request { set; get; }

        public GetLastTransactionsExtendedRequest()
        {
            Request = new GetLastTransactionsExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetLastTransactionsExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public int Count { set; get; }
    }
}