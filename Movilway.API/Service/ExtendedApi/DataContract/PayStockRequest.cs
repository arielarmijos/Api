using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped=false)]
    public class PayStockRequest : IMovilwayApiRequestWrapper<PayStockRequestBody>
    {
        [MessageBodyMember(Name = "PayStockRequest")]
        public PayStockRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "PayStockRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class PayStockRequestBody : ASecuredApiRequest 
    {
        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public Decimal Amount { set; get; }

        [Loggable]
        [DataMember(Order = 2, IsRequired = true)]
        public String BankName { set; get; }

        [Loggable]
        [DataMember(Order = 3, IsRequired = true)]
        public String TransactionReference { set; get; }

        [Loggable]
        [DataMember(Order = 4, IsRequired = false)]
        public String Description { set; get; }


    }
}