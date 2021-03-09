using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.Internal
{
    public class BalanceResponseInternal:ApiResponseInternal
    {
        public Decimal StockBalance { set; get; }
        public Decimal WalletBalance { set; get; }
        public Decimal PointsBalance { set; get; }
        public Decimal DebtBalance { set; get; }
    }
}