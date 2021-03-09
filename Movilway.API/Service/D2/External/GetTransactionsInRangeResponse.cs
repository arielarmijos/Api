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
    public class GetTransactionsInRangeResponse
    {
        [MessageBodyMember(Name = "GetTransactionsInRangeResponse", Namespace = "http://api.movilway.net/schema")]
        public GetTransactionsInRangeResponseBody Response { set; get; }

        public GetTransactionsInRangeResponse()
        {
            Response = new GetTransactionsInRangeResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetTransactionsInRangeResponseBody : ApiResponse
    {
        [DataMember]
        public TransactionSummaryLiteList Transactions { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema", ItemName="TransactionSummaryLite")]
    public class TransactionSummaryLiteList : List<TransactionSummaryLite> { }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TransactionSummaryLite
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public long TransactionID { set; get; }

        [DataMember(Order = 2, EmitDefaultValue=false)]
        public String Type { set; get; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public DateTime Date { set; get; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public Decimal Amount { set; get; }
    }
}