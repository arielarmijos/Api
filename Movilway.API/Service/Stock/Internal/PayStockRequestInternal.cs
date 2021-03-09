using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.Stock.Internal
{
    public class PayStockRequestInternal : ApiRequestInternal
    {
        public decimal Amount { set; get; }
        public String Bank { set; get; }
        public String Account { set; get; }
        public String Voucher { set; get; }
    }
}