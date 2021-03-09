using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;
using System.Text;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class GetLastTransactionsResponse : IMovilwayApiResponseWrapper<GetLastTransactionsResponseBody>
    {
        [MessageBodyMember(Name = "GetLastTransactionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public GetLastTransactionsResponseBody Response { set; get; }

        public GetLastTransactionsResponse()
        {
            Response = new GetLastTransactionsResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "GetLastTransactionsResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class GetLastTransactionsResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        [Loggable]
        [DataMember(Order = 3)]
        public TransactionList Transactions { set; get; }

        public override string ToString()
        {
            return Utils.logFormat(this);
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Transaction")]
    public class TransactionList : List<TransactionSummary>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //foreach (var item in this)
            //    sb.Append(Utils.logFormat(item) + ",");
            //if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append(String.Concat(this.GetType().Name, " Count ", this.Count));
            return sb.ToString();
        }
    }

    [CollectionDataContract(Namespace = "http://api.movilway.net/schema/extended", ItemName = "Party")]
    public class RelatedParties : List<String>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //foreach (var item in this)
            //    sb.Append("Party=" + item + ",");
            //sb.Remove(sb.Length - 1, 1);
            sb.Append(String.Concat(this.GetType().Name, " Count ", this.Count));
            return sb.ToString();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/extended")]
    public class TransactionSummary
    {
        [Loggable]
        [DataMember(Order = 0)]
        public long OriginalTransactionID { set; get; }

        [Loggable]
        [DataMember(Order = 1)]
        public String TransactionType { set; get; }

        [Loggable]
        [DataMember(Order = 2)]
        public DateTime LastTimeModified { set; get; }

        [Loggable]
        [DataMember(Order = 3)]
        public Decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 4)]
        public RelatedParties RelatedParties { set; get; }

        [Loggable]
        [DataMember(Order = 5)]
        public String Recipient { set; get; }
    }
}