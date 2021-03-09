using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class BuyStockExtendedResponse
    {
        [MessageBodyMember(Name = "BuyStockExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public BuyStockExtendedResponseBody Response { set; get; }

        public BuyStockExtendedResponse()
        {
            Response = new BuyStockExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BuyStockExtendedResponseBody : ApiResponse
    {

    }


}