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
    public class GetAgentGroupsResponse
    {
        [MessageBodyMember(Name = "GetAgentGroupsResponse", Namespace = "http://api.movilway.net/schema")]
        public GetAgentGroupsResponseBody Response { set; get; }

        public GetAgentGroupsResponse()
        {
            Response = new GetAgentGroupsResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class GetAgentGroupsResponseBody : ApiResponse
    {
        [DataMember]
        public GroupList GroupList { set; get; }
    }
}