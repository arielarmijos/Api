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
    public class SummaryResponse
    {
        [MessageBodyMember(Name = "SummaryResponse", Namespace = "http://api.movilway.net/schema")]
        public SummaryResponseBody Response { set; get; }

        public SummaryResponse()
        {
            Response = new SummaryResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class SummaryResponseBody : ApiResponse
    {
        [DataMember(Order = 1)]
        public int TransactionCount { set; get; }

        [DataMember(Order = 2)]
        public Decimal TotalAmount { set; get; }
    }
}