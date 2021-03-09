using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class PayStockResponse
    {
        [MessageBodyMember(Name = "PayStockResponse", Namespace = "http://api.movilway.net/schema")]
        public PayStockResponseBody Response { set; get; }

        public PayStockResponse()
        {
            Response = new PayStockResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class PayStockResponseBody : ApiResponse
    {
        [DataMember]
        public decimal Fee { set; get; }
    }

}