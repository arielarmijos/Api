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
    public class UnBlockAgentResponse : IMovilwayApiResponseWrapper<UnBlockAgentResponseBody>
    {
        [MessageBodyMember(Name = "UnBlockAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public UnBlockAgentResponseBody Response { set; get; }

        public UnBlockAgentResponse()
        {
            Response = new UnBlockAgentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "UnBlockAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class UnBlockAgentResponseBody : Common.AGenericApiResponse
    {

    }
}