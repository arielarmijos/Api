using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class BuyStockResponse
    {
        [MessageBodyMember(Name = "BuyStockResponse", Namespace = "http://api.movilway.net/schema")]
        public BuyStockResponseBody Response { set; get; }

        public BuyStockResponse()
        {
            Response = new BuyStockResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BuyStockResponseBody : ApiResponse
    {

    }


}