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
    public class GetAgentListResponse
    {
        [MessageBodyMember(Name = "GetAgentListResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAgentListResponseBody Response { set; get; }

        public GetAgentListResponse()
        {
            Response = new GetAgentListResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentListResponseBody : ApiResponse
    {
        [DataMember]
        public ApiKeyValuePair AgentList { set; get; }
    }
}