using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class BuyStockExtendedRequest
    {
        [MessageBodyMember(Name = "BuyStockeExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public BuyStockExtendedRequestBody Request { set; get; }

        public BuyStockExtendedRequest()
        {
            Request = new BuyStockExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class BuyStockExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(IsRequired = true)]
        public decimal Amount { set; get; }
    }
}