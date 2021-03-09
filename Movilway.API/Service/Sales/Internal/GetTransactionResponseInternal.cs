using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{
    public class GetTransactionResponseInternal : ApiResponseInternal
    {
        public int TransactionResult { set; get; }
        public String Recipient { set; get; }
        public Decimal Amount { set; get; }
        public DateTime TransactionDate { set; get; }
        public String TransactionType { set; get; }
        public String Initiator { set; get; }
        public String Debtor{ set; get; }
        public String Creditor { set; get; }
    }
}