using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movilway.API.Service.D2.Internal
{
    public class TransferRequestInternal : ApiRequestInternal
    {
        public Decimal Amount { set; get; }
        public String Recipient { set; get; }
    }
}