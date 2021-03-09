using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class TransferStockExtendedResponse
    {
        [MessageBodyMember(Name = "TransferStockExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public TransferStockExtendedResponseBody Response { set; get; }

        public TransferStockExtendedResponse()
        {
            Response = new TransferStockExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TransferStockExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public decimal Fee { set; get; }
    }


}