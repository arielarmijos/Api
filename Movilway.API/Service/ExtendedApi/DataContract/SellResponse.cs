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
    public class SellResponse : IMovilwayApiResponseWrapper<SellResponseBody>
    {
        [MessageBodyMember(Name = "SellResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public SellResponseBody Response { set; get; }

        public SellResponse()
        {
            Response = new SellResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "SellResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class SellResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AFinancialApiResponse
    {
        
    }
}