using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.Logging.Attribute;
using Movilway.API.Service.ExtendedApi.DataContract.Common;

namespace Movilway.API.Service.ExtendedApi.DataContract
{
    [MessageContract(IsWrapped = false)]
    public class CompleteBuyStockResponse:IMovilwayApiResponseWrapper<CompleteBuyStockResponseBody>
    {
        [MessageBodyMember(Name = "CompleteBuyStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public CompleteBuyStockResponseBody Response { set; get; }

        public CompleteBuyStockResponse()
        {
            Response = new CompleteBuyStockResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "CompleteBuyStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class CompleteBuyStockResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {

    }
}