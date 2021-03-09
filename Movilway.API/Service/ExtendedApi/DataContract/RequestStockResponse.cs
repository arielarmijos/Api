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
    public class RequestStockResponse : IMovilwayApiResponseWrapper<RequestStockResponseBody>
    {
        [MessageBodyMember(Name = "RequestStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public RequestStockResponseBody Response { set; get; }

        public RequestStockResponse()
        {
            Response = new RequestStockResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "RequestStockResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class RequestStockResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {
        
    }
}