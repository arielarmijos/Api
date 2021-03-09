using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class BuyStockRequest
    {
        [MessageBodyMember(Name = "BuyStockRequest", Namespace = "http://api.movilway.net/schema/oldapi")]
        public BuyStockRequestBody Request { set; get; }

        public BuyStockRequest()
        {
            Request = new BuyStockRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema/oldapi")]
    public class BuyStockRequestBody : SessionApiRequest
    {
        [DataMember(IsRequired = true)]
        public int DeviceType { set; get; }

        [DataMember(IsRequired = true)]
        public decimal Amount { set; get; }
    }
}