using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.D2.External
{

    [MessageContract(IsWrapped = false)]
    public class GetLastTransactionsRequest
    {
        [MessageBodyMember(Name = "GetLastTransactionRequest", Namespace = "http://api.movilway.net/schema")]
        public GetLastTransactionsRequestBody Request { set; get; }

        public GetLastTransactionsRequest()
        {
            Request = new GetLastTransactionsRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetLastTransactionsRequestBody : SessionApiRequest
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(Order = 2, IsRequired = true)]
        public String Agent { set; get; }

        [DataMember(Order = 3, IsRequired = true)]
        public int Count { set; get; }
    }
}