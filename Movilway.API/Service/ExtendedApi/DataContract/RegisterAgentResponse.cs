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
    public class RegisterAgentResponse : IMovilwayApiResponseWrapper<RegisterAgentResponseBody>
    {
        [MessageBodyMember(Name = "RegisterAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
        public RegisterAgentResponseBody Response { set; get; }

        public RegisterAgentResponse()
        {
            Response = new RegisterAgentResponseBody();
        }
    }

    [Loggable]
    [DataContract(Name = "RegisterAgentResponse", Namespace = "http://api.movilway.net/schema/extended")]
    public class RegisterAgentResponseBody : Movilway.API.Service.ExtendedApi.DataContract.Common.AGenericApiResponse
    {

    }
}