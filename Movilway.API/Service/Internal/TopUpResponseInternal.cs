using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.Internal
{
    public class TopUpResponseInternal:ApiResponseInternal
    {
        public String HostTransRef { set; get; }
        public Decimal Fee { set; get; }
        public Decimal BalanceStock { set; get; }
    }
}