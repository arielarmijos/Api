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
    public class PayStockResponse : IMovilwayApiResponseWrapper<PayStockResponseBody>
    {
        [MessageBodyMember(Name = "PayStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public PayStockResponseBody Response { set; get; }

        public PayStockResponse()
        {
            Response = new PayStockResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "PayStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class PayStockResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {

    }
}