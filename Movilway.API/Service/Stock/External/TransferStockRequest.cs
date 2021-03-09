using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class TransferStockRequest
    {
        [MessageBodyMember(Name = "TransferStockRequest", Namespace = "http://api.movilway.net/schema")]
        public TransferStockRequestBody Request { set; get; }

        public TransferStockRequest()
        {
            Request = new TransferStockRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TransferStockRequestBody : SessionApiRequest
    {
        [DataMember(IsRequired = true, Order=1)]
        public int DeviceType { set; get; }

        [DataMember(IsRequired = true, Order = 2)]
        public decimal Amount { set; get; }

        [DataMember(IsRequired = true, Order = 3)]
        public String Agent { set; get; }
    }
}