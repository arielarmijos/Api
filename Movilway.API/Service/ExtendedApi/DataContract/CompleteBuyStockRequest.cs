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
    public class CompleteBuyStockRequest : IMovilwayApiRequestWrapper<CompleteBuyStockRequestBody>
    {
        [MessageBodyMember(Name = "CompleteBuyStockRequest")]
        public CompleteBuyStockRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "CompleteBuyStockRequest", Namespace = "http://api.movilway.net/schema/extended")]
    public class CompleteBuyStockRequestBody : ASecuredApiRequest
    {
        [Loggable]
        [DataMember(Order = 1, IsRequired = true)]
        public int BuyStockTransactionID { set; get; }

    }
}