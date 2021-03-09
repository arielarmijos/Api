using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class TransferStockExtendedRequest
    {
        [MessageBodyMember(Name = "TransferStockExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public TransferStockExtendedRequestBody Request { set; get; }

        public TransferStockExtendedRequest()
        {
            Request = new TransferStockExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class TransferStockExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(IsRequired = true, Order=1)]
        public int DeviceType { set; get; }

        [DataMember(IsRequired = true, Order = 2)]
        public decimal Amount { set; get; }

        [DataMember(IsRequired = true, Order = 3)]
        public String Agent { set; get; }
    }
}