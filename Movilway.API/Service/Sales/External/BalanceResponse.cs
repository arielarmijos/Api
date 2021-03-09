using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Sales.External
{
    [MessageContract(IsWrapped = false)]
    public class BalanceResponse
    {
        [MessageBodyMember(Name = "BalanceResponse", Namespace = "http://api.movilway.net/schema")]
        public BalanceResponseBody Response { set; get; }

        public BalanceResponse()
        {
            Response = new BalanceResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BalanceResponseBody : ApiResponse
    {
       
        [DataMember(Order = 1)]
        public Decimal Balance{ set; get; }
    }
}