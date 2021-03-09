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
    public class GetAgentByReferenceExtendedResponse
    {
        [MessageBodyMember(Name = "GetAgentByReferenceExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAgentByReferenceExtendedResponseBody Response { set; get; }

        public GetAgentByReferenceExtendedResponse()
        {
            Response = new GetAgentByReferenceExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentByReferenceExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public AgentInfo AgentInfo { set; get; }
    }
}