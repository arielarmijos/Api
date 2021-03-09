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
    public class SummaryExtendedResponse
    {
        [MessageBodyMember(Name = "SummaryExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public SummaryExtendedResponseBody Response { set; get; }

        public SummaryExtendedResponse()
        {
            Response = new SummaryExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SummaryExtendedResponseBody : ApiResponse
    {
        [DataMember(Order = 1)]
        public int TransactionCount { set; get; }

        [DataMember(Order = 2)]
        public Decimal TotalAmount { set; get; }
    }
}