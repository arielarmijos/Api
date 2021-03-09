using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Stock.External
{

    [MessageContract(IsWrapped = false)]
    public class PayStockExtendedRequest
    {
        [MessageBodyMember(Name = "PayStockExtendedRequest", Namespace = "http://api.movilway.net/schema")]
        public PayStockExtendedRequestBody Request { set; get; }

        public PayStockExtendedRequest()
        {
            Request = new PayStockExtendedRequestBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class PayStockExtendedRequestBody : ExtendedApiRequest
    {
        [DataMember(IsRequired = true, Order=1)]
        public int DeviceType { set; get; }

        [DataMember(IsRequired = true, Order = 2)]
        public decimal Amount { set; get; }

        [DataMember(IsRequired = true, Order = 3)]
        public String Bank { set; get; }

        [DataMember(IsRequired = true, Order = 4)]
        public String Account { set; get; }

        [DataMember(IsRequired = true, Order = 5)]
        public String Voucher { set; get; }
    }
}