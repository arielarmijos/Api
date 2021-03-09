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
    public class GetAllAgentGroupsResponse
    {
        [MessageBodyMember(Name = "GetAllAgentGroupsResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAllAgentGroupsResponseBody Response { set; get; }

        public GetAllAgentGroupsResponse()
        {
            Response = new GetAllAgentGroupsResponseBody();
        }
    }

    

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAllAgentGroupsResponseBody : ApiResponse
    {
        [DataMember]
        public GroupList AllGroups { set; get; }
    }
}