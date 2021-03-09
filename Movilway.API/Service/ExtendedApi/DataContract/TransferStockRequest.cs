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
    public class TransferStockRequest : IMovilwayApiRequestWrapper<TransferStockRequestBody>
    {
        [MessageBodyMember(Name = "TransferStockRequest")]
        public TransferStockRequestBody Request { set; get; }
    }

    [Loggable]
    [DataContract(Name = "TransferStockRequest", Namespace="http://api.movilway.net/schema/extended")]
    public class TransferStockRequestBody : ASecuredFinancialApiRequest
    {
        [Loggable]
        [DataMember(IsRequired = true, Order = 3)]
        public String Agent { set; get; }
    }
}