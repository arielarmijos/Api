using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetTransactionsInRangeResponse : IMovilwayApiResponseWrapper<GetTransactionsInRangeResponseBody>
    {
        [MessageBodyMember(Name = "GetTransactionsInRangeResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetTransactionsInRangeResponseBody Response { set; get; }

        public GetTransactionsInRangeResponse()
        {
            Response = new GetTransactionsInRangeResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetTransactionsInRangeResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetTransactionsInRangeResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [DataMember]
        public TransactionSummaryLiteList Transactions { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "TransactionSummaryLite")]
    public class TransactionSummaryLiteList : List<TransactionSummaryLite> { }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class TransactionSummaryLite
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public long OriginalTransactionID { set; get; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public String Type { set; get; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public DateTime Date { set; get; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public Decimal Amount { set; get; }
    }
}