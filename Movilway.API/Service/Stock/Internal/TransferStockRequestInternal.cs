using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.Stock.Internal
{
    public class TransferStockRequestInternal : ApiRequestInternal
    {
        public Decimal Amount { set; get; }
        public String Agent { set; get; }
    }
}