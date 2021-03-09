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
    public class UnMapAgentToGroupResponse
    {
        [MessageBodyMember(Name = "UnMapAgentToGroupResponse", Namespace = "http://api.movilway.net/schema")]
        public UnMapAgentToGroupResponseBody Response { set; get; }

        public UnMapAgentToGroupResponse()
        {
            Response = new UnMapAgentToGroupResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class UnMapAgentToGroupResponseBody : ApiResponse
    {

    }
}