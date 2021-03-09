using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Registration.External
{
    [MessageContract(IsWrapped = false)]
    public class RegisterAgentResponse
    {
        [MessageBodyMember(Name = "RegisterAgentResponse", Namespace = "http://api.movilway.net/schema")]
        public RegisterAgentResponseBody Response { set; get; }

        public RegisterAgentResponse()
        {
            Response = new RegisterAgentResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class RegisterAgentResponseBody : ApiResponse
    {

    }
}