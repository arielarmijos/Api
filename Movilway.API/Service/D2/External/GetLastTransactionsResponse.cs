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
    public class GetLastTransactionsResponse
    {
        [MessageBodyMember(Name = "GetLastTransactionsResponse", Namespace = "http://api.movilway.net/schema")]
        public GetLastTransactionsResponseBody Response { set; get; }

        public GetLastTransactionsResponse()
        {
            Response = new GetLastTransactionsResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetLastTransactionsResponseBody : ApiResponse
    {
        [DataMember]
        public List<TransactionSummary> Transactions { set; get; }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema")]
    public class GroupList : List<TransactionSummary> { }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema")]
    public class ArrayOfString : List<String> { }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TransactionSummary
    {
        [DataMember(Order=1)]
        public long TransactionID { set; get; }

        [DataMember(Order = 2)]
        public String TransactionType { set; get; }

        [DataMember(Order = 3)]
        public DateTime LastTimeModified { set; get; }

        [DataMember(Order = 4)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 5)]
        public ArrayOfString PartiesReferenceIDList { set; get; }

    }
}