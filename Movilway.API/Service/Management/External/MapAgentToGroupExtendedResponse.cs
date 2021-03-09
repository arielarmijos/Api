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
    public class MapAgentToGroupExtendedResponse
    {
        [MessageBodyMember(Name = "MapAgentToGroupExtendedResponse", Namespace = "http://api.movilway.net/schema")]
        public MapAgentToGroupExtendedResponseBody Response { set; get; }

        public MapAgentToGroupExtendedResponse()
        {
            Response = new MapAgentToGroupExtendedResponseBody();
        }
    }

    [DataContract(Namespace = "http://api.movilway.net/schema")]
    public class MapAgentToGroupExtendedResponseBody : ApiResponse
    {

    }
}