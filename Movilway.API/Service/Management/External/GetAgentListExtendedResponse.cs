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
    public class GetAgentListExtendedResponse
    {
        [MessageBodyMember(Name = "GetAgentListExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAgentListExtendedResponseBody Response { set; get; }

        public GetAgentListExtendedResponse()
        {
            Response = new GetAgentListExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentListExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public ApiKeyValuePair AgentList { set; get; }
    }
}