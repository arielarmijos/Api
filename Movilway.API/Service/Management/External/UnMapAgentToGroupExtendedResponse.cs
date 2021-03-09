using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;
using Movilway.API.Service.External;

namespace Movilway.API.Service.Management.External
{
    [MessageContract(IsWrapped = false)]
    public class UnMapAgentToGroupExtendedResponse
    {
        [MessageBodyMember(Name = "UnMapAgentToGroupExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public UnMapAgentToGroupExtendedResponseBody Response { set; get; }

        public UnMapAgentToGroupExtendedResponse()
        {
            Response = new UnMapAgentToGroupExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class UnMapAgentToGroupExtendedResponseBody : ApiResponse
    {

    }
}