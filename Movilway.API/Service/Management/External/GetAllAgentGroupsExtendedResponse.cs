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
    public class GetAllAgentGroupsExtendedResponse
    {
        [MessageBodyMember(Name = "GetAllAgentGroupsExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAllAgentGroupsExtendedResponseBody Response { set; get; }

        public GetAllAgentGroupsExtendedResponse()
        {
            Response = new GetAllAgentGroupsExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAllAgentGroupsExtendedResponseBody : ApiResponse
    {
        [DataMember]
        public GroupList AllGroups { set; get; }
    }
}