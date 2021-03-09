using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{
    [MessageContract(IsWrapped = false)]
    public class BalanceExtendedResponse
    {
        [MessageBodyMember(Name = "BalanceExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public BalanceExtendedResponseBody Response { set; get; }

        public BalanceExtendedResponse()
        {
            Response = new BalanceExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BalanceExtendedResponseBody : ApiResponse
    {
        [DataMember(Order = 1)]
        public Decimal Balance { set; get; }
    }
}