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
    public class MapAgentToGroupResponse
    {
        [MessageBodyMember(Name = "MapAgentToGroupResponse", Namespace = "http://api.movilway.net/schema")]
        public MapAgentToGroupResponseBody Response { set; get; }

        public MapAgentToGroupResponse()
        {
            Response = new MapAgentToGroupResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class MapAgentToGroupResponseBody : ApiResponse
    {

    }
}