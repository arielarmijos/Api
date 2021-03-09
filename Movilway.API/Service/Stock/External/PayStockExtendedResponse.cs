using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class PayStockExtendedResponse
    {
        [MessageBodyMember(Name = "PayStockExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public PayStockExtendedResponseBody Response { set; get; }

        public PayStockExtendedResponse()
        {
            Response = new PayStockExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class PayStockExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public decimal Fee { set; get; }
    }

}