using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.Stock.Internal
{
    public class PayStockResponseInternal : ApiResponseInternal
    {
        public decimal Fee { set; get; }
    }
}