using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.D2.External
{
    [MessageContract(IsWrapped = false)]
    public class GetLastTransactionsExtendedResponse
    {
        [MessageBodyMember(Name = "GetLastTransactionsExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetLastTransactionsExtendedResponseBody Response { set; get; }

        public GetLastTransactionsExtendedResponse()
        {
            Response = new GetLastTransactionsExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetLastTransactionsExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public List<TransactionSummary> Transactions { set; get; }
    }
}