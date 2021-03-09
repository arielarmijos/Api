using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.ExtendedApi.DataContract.Common;
using Movilway.Logging.Attribute;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class TransferStockResponse : IMovilwayApiResponseWrapper<TransferStockResponseBody>
    {
        [MessageBodyMember(Name = "TransferStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public TransferStockResponseBody Response { set; get; }

        public TransferStockResponse()
        {
            Response = new TransferStockResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "TransferStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class TransferStockResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {

    }
}