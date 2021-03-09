using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{

    [MessageContract(IsWrapped = false)]
    public class GetTransactionExtendedResponse
    {
        [MessageBodyMember(Name = "GetTransactionExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetTransactionExtendedResponseBody Response { set; get; }

        public GetTransactionExtendedResponse()
        {
            Response = new GetTransactionExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetTransactionExtendedResponseBody : ApiResponse
    {
        [DataMember(Order = 0)]
        public int TransactionResult { set; get; }

        [DataMember(Order = 1)]
        public String Recipient { set; get; }

        [DataMember(Order = 2)]
        public Decimal Amount { set; get; }

        [DataMember(Order = 3)]
        public DateTime TransactionDate { set; get; }

        [DataMember(Order = 4)]
        public String TransactionType { set; get; }

        [DataMember(Order = 5)]
        public String Initiator { set; get; }

        [DataMember(Order = 6)]
        public String Debtor { set; get; }

        [DataMember(Order = 7)]
        public String Creditor { set; get; }
    }
}