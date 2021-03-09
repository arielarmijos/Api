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
    public class GetAgentGroupsExtendedResponse
    {
        [MessageBodyMember(Name = "GetAgentGroupsExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAgentGroupsExtendedResponseBody Response { set; get; }

        public GetAgentGroupsExtendedResponse()
        {
            Response = new GetAgentGroupsExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentGroupsExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public GroupList GroupList { set; get; }
    }
}