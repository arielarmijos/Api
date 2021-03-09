using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.D2.Internal
{
    public class GetLastTransactionsResponseInternal : ApiResponseInternal
    {
        public List<TransactionSummaryInternal> Transactions { set; get; }
    }

    public class TransactionSummaryInternal
    {
        public long TransactionID { set; get; }
        public String TransactionType { set; get; }
        public DateTime LastTimeModified { set; get; }
        public Decimal Amount { set; get; }
        public String[] PartiesReferenceIDList { set; get; }

    }
}