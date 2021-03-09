using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class TransferStockResponse
    {
        [MessageBodyMember(Name = "TransferStockResponse", Namespace = "http://api.movilway.net/schema")]
        public TransferStockResponseBody Response { set; get; }

        public TransferStockResponse()
        {
            Response = new TransferStockResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TransferStockResponseBody : ApiResponse
    {
        [DataMember]
        public decimal Fee { set; get; }
    }


}