using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Movilway.API.Service.Registration.External
{
    [MessageContract(IsWrapped = false)]
    public class RegisterAgentBulkResponse
    {
        [MessageBodyMember(Name = "RegisterAgentBulkResponse", Namespace = "http://api.movilway.net/schema")]
        public RegisterAgentBulkResponseBody Response { set; get; }

        public RegisterAgentBulkResponse()
        {
            Response = new RegisterAgentBulkResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class RegisterAgentBulkResponseBody : ApiResponse
    {

    }
}