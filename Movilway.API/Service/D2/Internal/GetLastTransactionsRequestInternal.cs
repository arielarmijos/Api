using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.D2.Internal
{
    public class GetLastTransactionsRequestInternal : ApiRequestInternal
    {
        public String Agent { set; get; }
        public int Count { set; get; }
    }
}

