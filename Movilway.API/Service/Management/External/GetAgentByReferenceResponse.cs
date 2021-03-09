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
    public class GetAgentByReferenceResponse
    {
        [MessageBodyMember(Name = "GetAgentByReferenceResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAgentByReferenceResponseBody Response { set; get; }

        public GetAgentByReferenceResponse()
        {
            Response = new GetAgentByReferenceResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentByReferenceResponseBody : ApiResponse
    {
        [DataMember]
        public AgentInfo AgentInfo { set; get; }
    }


}